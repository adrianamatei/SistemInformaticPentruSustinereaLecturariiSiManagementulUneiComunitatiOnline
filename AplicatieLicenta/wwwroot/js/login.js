document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('container');
    const signInReaderButton = document.getElementById('signInReader');
    const signInAdminButton = document.getElementById('signInAdmin');

    signInReaderButton.addEventListener('click', () => {
        container.classList.remove('right-panel-active');
    });

    signInAdminButton.addEventListener('click', () => {
        container.classList.add('right-panel-active');
    });
});
