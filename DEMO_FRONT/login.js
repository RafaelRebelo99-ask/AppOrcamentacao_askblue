const showPassBtn = document.getElementById('show-password');
const passwordInput = document.getElementById('password');
let passwordVisible = false;

showPassBtn.addEventListener('click', () => {
    passwordVisible = !passwordVisible;

    passwordInput.type = passwordVisible ? 'text' : 'password';
    showPassBtn.classList.toggle('bi-eye', !passwordVisible);
    showPassBtn.classList.toggle('bi-eye-slash', passwordVisible);
});

const checkbox = document.getElementById('remember-me');

checkbox.addEventListener('click', () => {
    checkbox.classList.toggle('selected');
});