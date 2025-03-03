let currentOpenId = null;

async function toggleUserForm(userId) {

    const collapseRow = document.getElementById(`row_${userId}`);
    const formContent = document.getElementById(`formContent_${userId}`);
    const contentWrapper = document.getElementById(`content_${userId}`);

    if (currentOpenId === userId) {
        contentWrapper.classList.remove('show');
        setTimeout(() => {
            bootstrap.Collapse.getOrCreateInstance(collapseRow).hide();
        }, 300);
        bootstrap.Collapse.getOrCreateInstance(collapseRow).toggle();
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
        contentWrapper.classList.remove('show');
        bootstrap.Collapse.getOrCreateInstance(collapseRow).show();

        setTimeout(() => {
            contentWrapper.classList.add('show');
        }, 100);
        currentOpenId = userId;

    } catch (error) {
        console.error('Error loading form', error);
    }
}
