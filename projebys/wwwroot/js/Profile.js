// Alt menü aç/kapa fonksiyonu
function toggleSubMenu(menuId) {
    // Seçilen alt menüyü bul
    const menu = document.getElementById(menuId);

    // Tüm alt menüleri kapat
    document.querySelectorAll('.sub-menu').forEach(subMenu => {
        if (subMenu !== menu) {
            subMenu.style.display = 'none';
        }
    });

    // Seçilen alt menüyü aç/kapa
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}
function toggleSubMenu(menuId) {
    const menu = document.getElementById(menuId);
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}

document.addEventListener('DOMContentLoaded', function () {
    // Kullanıcı bilgilerini yükle
    fetch('/api/Student/getStudentInfo', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Kullanıcı bilgileri alınamadı.");
            }
            return response.json();
        })
        .then(data => {
            // Data doğru geldiği durumda bilgileri sayfada göster
            if (data && data.studentInfo) {
                // Öğrenci bilgilerini form alanlarına doldur
                document.getElementById('firstName').value = data.studentInfo.firstName;
                document.getElementById('lastName').value = data.studentInfo.lastName;
                document.getElementById('email').value = data.email;

                // Öğrenci ID'sini al
                const studentId = data.studentInfo.studentID;

                // Linkleri dinamik olarak ayarla
                document.getElementById('updateProfileLink').addEventListener('click', function (event) {
                    event.preventDefault();
                    window.location.href = `/students/updateProfile/${studentId}`;
                });

                document.getElementById('viewTranscriptLink').addEventListener('click', function (event) {
                    event.preventDefault();
                    window.location.href = `/students/transcript/${studentId}`;
                });

                document.getElementById('selectCoursesLink').addEventListener('click', function (event) {
                    event.preventDefault();
                    window.location.href = `/students/courses/${studentId}`;
                });

                document.getElementById('myCoursesLink').addEventListener('click', function (event) {
                    event.preventDefault();
                    window.location.href = `/students/MyCourses/${studentId}`;
                });

                // Form gönderme işlemi
                document.getElementById('updateProfileForm').addEventListener('submit', function (event) {
                    event.preventDefault();

                    // Formdan alınan veriler
                    const updatedData = {
                        studentId: studentId,  // Öğrenci ID'sini ekliyoruz
                        firstName: document.getElementById('firstName').value,
                        lastName: document.getElementById('lastName').value,
                        email: document.getElementById('email').value,
                    };

                    // Öğrenci bilgilerini güncelleme API çağrısı
                    fetch(`/api/student/updateProfile/${studentId}`, {
                        method: 'PUT',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(updatedData)
                    })
                        .then(response => {
                            if (!response.ok) {
                                throw new Error("Güncelleme sırasında bir hata oluştu.");
                            }
                            return response.json();
                        })
                        .then(data => {
                            alert('Profil başarıyla güncellendi.');
                        })
                        .catch(error => {
                            console.error('Güncelleme sırasında hata oluştu:', error);
                            alert('Güncelleme sırasında bir hata oluştu.');
                        });
                });
            } else {
                console.error("Öğrenci bilgileri eksik veya hatalı");
                alert("Öğrenci bilgileri yüklenirken bir hata oluştu.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Kullanıcı bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser';
        });
});