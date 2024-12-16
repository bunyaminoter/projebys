document.addEventListener('DOMContentLoaded', function () {
    const studentTab = document.querySelector('.tab.student');
    const personnelTab = document.querySelector('.tab.personnel');
    const loginForm = document.querySelector('.login-form');
    let selectedRole = 'Student'; // Varsayılan olarak "Student"

    // Sekmeler arası geçiş yapıldığında aktif rolü belirle
    studentTab.addEventListener('click', function () {
        setActiveTab('Student');
    });

    personnelTab.addEventListener('click', function () {
        setActiveTab('Personnel');
    });

    function setActiveTab(role) {
        if (role === 'Student') {
            studentTab.classList.add('active');
            personnelTab.classList.remove('active');
            selectedRole = 'Student'; // Aktif rolü "Student" olarak ayarla
        } else if (role === 'Personnel') {
            personnelTab.classList.add('active');
            studentTab.classList.remove('active');
            selectedRole = 'Personnel'; // Aktif rolü "Personnel" olarak ayarla
        }
    }

    // Login formunu gönderme
    document.getElementById("login-form").addEventListener("submit", function (event) {
        event.preventDefault();

        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;

        if (!email || !password) {
            document.getElementById("error-message").textContent = "Lütfen tüm alanları doldurun.";
            document.getElementById("error-message").style.display = "block";
            return;
        }

        submitLogin(email, password, selectedRole); // Kullanıcı rolü ile birlikte gönder
    });

    async function submitLogin(email, password) {
        try {
            // Aktif sekmeyi belirle
            const isStudentActive = document.querySelector('.tab.student').classList.contains('active');
            const selectedRole = isStudentActive ? 'Student' : 'Personnel'; // Student veya Personnel olarak belirleniyor

            const response = await fetch('/api/Login/authenticate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    Email: email,
                    PasswordHash: password,
                    SelectedRole: selectedRole // Kullanıcının seçtiği rol
                })
            });

            if (response.ok) {
                const data = await response.json();

                // Aktif sekmeye göre yönlendirme yap
                if (selectedRole === 'Student') {
                    window.location.href = '/Student/StudentDashboard';
                } else {
                    window.location.href = '/Personnel/PersonnelDashboard';
                }
            } else if (response.status === 401) {
                document.getElementById("error-message").textContent = "E-posta veya şifre yanlış.";
                document.getElementById("error-message").style.display = "block";
            } else if (response.status === 403) {
                document.getElementById("error-message").textContent = "Lütfen kendi alanınızdan giriş yapınız.";
                document.getElementById("error-message").style.display = "block";
            } else {
                document.getElementById("error-message").textContent = "Bir hata oluştu. Lütfen tekrar deneyin.";
                document.getElementById("error-message").style.display = "block";
            }
        } catch (error) {
            document.getElementById("error-message").textContent = "Sunucuya bağlanırken bir hata oluştu.";
            document.getElementById("error-message").style.display = "block";
            console.error("Hata:", error);
        }
    }

});