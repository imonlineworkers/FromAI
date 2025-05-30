// user.js

let currentOpenId = null;

document.addEventListener('DOMContentLoaded', function () {
    // Handle form submit for Add and Edit User
    window.submitUserForm = function (event) {
        event.preventDefault();
        const form = event.target;
        const formData = new FormData(form);
        const userData = {};
        formData.forEach((value, key) => userData[key] = value);

        const isEdit = form.querySelector('input[name="IsEdit"]').value === 'true';
        const url = isEdit ? '/User/UpdateUser' : '/User/AddUser';

        fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        })
            .then(response => response.json())
            .then(data => {
                if (data.isSuccess) {
                    form.reset();
                    if (!isEdit) {
                        closeAddUserForm();
                    } else if (currentOpenId) {
                        closeEditUserForm(currentOpenId);
                    }
                    location.reload();
                } else {
                    alert('Failed to save user');
                }
            })
            .catch(error => console.error('Error:', error));
    };

    // Edit User
    window.toggleUserForm = async function (userId) {
        const collapseRow = document.getElementById(`row_${userId}`);
        const formContent = document.getElementById(`formContent_${userId}`);
        const contentWrapper = document.getElementById(`content_${userId}`);

        if (currentOpenId === userId) {
            closeEditUserForm(userId);
            return;
        }

        if (currentOpenId) {
            closeEditUserForm(currentOpenId);
        }

        try {
            const response = await fetch(`/User/EditUser/${userId}`);
            if (!response.ok) {
                console.error('Failed to load form');
                return;
            }

            formContent.innerHTML = await response.text();
            const isEditInput = document.createElement('input');
            isEditInput.type = 'hidden';
            isEditInput.name = 'IsEdit';
            isEditInput.value = 'true';
            formContent.querySelector('form').appendChild(isEditInput);

            bootstrap.Collapse.getOrCreateInstance(collapseRow).show();
            setTimeout(() => {
                contentWrapper.classList.add('show');
            }, 100);
            currentOpenId = userId;
        } catch (error) {
            console.error('Error loading form', error);
        }
    };

    // Delete User
    window.deleteUser = function (userId) {
        console.log(userId);
        if (!confirm('Are you sure you want to delete this user?')) return;

        fetch(`/User/DeleteUser`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userId)
        })
            .then(response => response.json())
            .then(data => {
                if (data.isSuccess) {
                    location.reload();
                } else {
                    alert('Failed to delete user');
                }
            })
            .catch(error => console.error('Error deleting user:', error));
    };

    window.closeForm = function (isEdit, userId) {
        console.log(userId);
        if (isEdit === false) {
            bootstrap.Collapse.getOrCreateInstance(document.getElementById('addUserForm')).hide();
        } else {
            closeEditUserForm(userId);
        }
    };

    window.closeEditUserForm = function (userId) {
        const collapseRow = document.getElementById(`row_${userId}`);
        const contentWrapper = document.getElementById(`content_${userId}`);
        contentWrapper.classList.remove('show');
        setTimeout(() => {
            bootstrap.Collapse.getOrCreateInstance(collapseRow).hide();
        }, 300);
        currentOpenId = null;

    };
});
