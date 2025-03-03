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

        const isEdit = !!userData.UserId;
        const url = isEdit ? '/User/UpdateUser' : '/User/AddUser';

        fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                form.reset();
                if (!isEdit) {
                    bootstrap.Collapse.getOrCreateInstance(document.getElementById('addUserForm')).hide();
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
            contentWrapper.classList.remove('show');
            setTimeout(() => {
                bootstrap.Collapse.getOrCreateInstance(collapseRow).hide();
            }, 300);
            currentOpenId = null;
            return;
        }

        if (currentOpenId) {
            const prevContent = document.getElementById(`content_${currentOpenId}`);
            const prevRow = document.getElementById(`row_${currentOpenId}`);
            prevContent.classList.remove('show');
            setTimeout(() => {
                bootstrap.Collapse.getOrCreateInstance(prevRow).hide();
            }, 300);
        }

        try {
            const response = await fetch(`/User/EditUser/${userId}`);
            if (!response.ok) {
                console.error('Failed to load form');
                return;
            }

            formContent.innerHTML = await response.text();
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
        if (!confirm('Are you sure you want to delete this user?')) return;

        fetch(`/User/DeleteUser`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById(`row_${userId}`).remove();
            } else {
                alert('Failed to delete user');
            }
        })
        .catch(error => console.error('Error deleting user:', error));
    };
});
