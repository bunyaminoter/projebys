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
    fetch('/api/Student/getStudentInfo')
        .then(response => {
            if (!response.ok) {
                throw new Error("Kullanıcı bilgileri alınamadı.");
            }
            return response.json();
        })
        .then(data => {
            if (data && data.studentInfo) {
                const studentId = data.studentInfo.studentID;

                // Linkleri dinamik olarak ayarla
                document.getElementById('updateProfileLink').href = `/students/updateProfile/${studentId}`;
                document.getElementById('viewTranscriptLink').href = `/students/transcript/${studentId}`;
                document.getElementById('selectCoursesLink').href = `/students/courses/${studentId}`;
                document.getElementById('myCoursesLink').href = `/students/MyCourses/${studentId}`;

                // Derslerim listesini doldur
                fetch(`/api/Student/MyCourses/${studentId}`)
                    .then(response => response.json())
                    .then(courses => {
                        const myCoursesTable = document.getElementById('myCoursesTable').getElementsByTagName('tbody')[0];
                        courses.forEach(course => {
                            const row = myCoursesTable.insertRow();
                            row.innerHTML = `
                           <td>${course.courseName}</td>
                             <td>${course.credit}</td>
                             <td>${course.isMandatory ? 'Zorunlu' : 'Seçmeli'}</td>
    `;
                        });

                    })
                    .catch(error => console.error('Derslerim listesi yüklenirken hata oluştu:', error));
            } else {
                alert("Öğrenci bilgileri eksik.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Kullanıcı bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser';
        });
});