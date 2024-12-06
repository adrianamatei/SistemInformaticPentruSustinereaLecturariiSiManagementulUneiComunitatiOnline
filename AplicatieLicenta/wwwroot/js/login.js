const container = document.getElementById('container');
const signInReaderButton = document.getElementById('signInReader');
const signInAdminButton = document.getElementById('signInAdmin');

// Activează formularul de cititor
signInReaderButton.addEventListener('click', () => {
    container.classList.remove('right-panel-active');
});

// Activează formularul de administrator
signInAdminButton.addEventListener('click', () => {
    container.classList.add('right-panel-active');
});
