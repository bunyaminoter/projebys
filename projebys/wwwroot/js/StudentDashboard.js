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
                document.getElementById('studentName').textContent = `${data.studentInfo.firstName} ${data.studentInfo.lastName}`;
                document.getElementById('studentEmail').textContent = data.email;
                document.getElementById('studentNumber').textContent = data.studentInfo.studentID;
                document.getElementById('studentDepartment').textContent = data.studentInfo.department || "Bilgisayar Mühendisliği";

                // Öğrenci ID'sini al ve URL'leri dinamik olarak oluştur
                const studentId = data.studentInfo.studentID;

                // Eğer linklere tıklama olayı dinlemek istiyorsanız:
                document.getElementById('updateProfileLink').addEventListener('click', function (event) {
                    event.preventDefault();  // Varsayılan davranışı engelle (linke tıklama)
                    window.location.href = `/students/updateProfile/${studentId}`;  // Yönlendirme yap
                });

                document.getElementById('viewTranscriptLink').addEventListener('click', function (event) {
                    event.preventDefault();  // Varsayılan davranışı engelle
                    window.location.href = `/students/transcript/${studentId}`;  // Yönlendirme yap
                });

                document.getElementById('selectCoursesLink').addEventListener('click', function (event) {
                    event.preventDefault();  // Varsayılan davranışı engelle
                    window.location.href = `/students/courses/${studentId}`;  // Yönlendirme yap
                });

                document.getElementById('myCoursesLink').addEventListener('click', function (event) {
                    event.preventDefault();  // Varsayılan davranışı engelle
                    window.location.href = `/students/MyCourses/${studentId}`;  // Yönlendirme yap
                });
            } else {
                // Veriler eksikse veya null dönerse, kullanıcıyı bilgilendir
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