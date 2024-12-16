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
            if (data && data.studentInfo) {
                const studentId = data.studentInfo.studentID;

                // Linkleri dinamik olarak ayarla
                document.getElementById('updateProfileLink').href = `/students/updateProfile/${studentId}`;
                document.getElementById('viewTranscriptLink').href = `/students/transcript/${studentId}`;
                document.getElementById('selectCoursesLink').href = `/students/courses/${studentId}`;
                document.getElementById('myCoursesLink').href = `/students/MyCourses/${studentId}`;

                // Ders listesini doldur
                fetch(`/api/Student/courses/${studentId}`)
                    .then(response => response.json())
                    .then(courses => {
                        const coursesTable = document.getElementById('coursesTable').getElementsByTagName('tbody')[0];
                        courses.forEach(course => {
                            const row = coursesTable.insertRow();
                            row.innerHTML = `
                                        <td>${course.code}</td>
                                        <td>${course.name}</td>
                                        <td>${course.credits}</td>
                                        <td><input type="checkbox" name="selectedCourses" value="${course.id}"></td>
                                    `;
                        });
                    });

                // Ders seçme formu gönderme
                document.getElementById('courseSelectionForm').addEventListener('submit', function (event) {
                    event.preventDefault();
                    const selectedCourses = Array.from(document.querySelectorAll('input[name="selectedCourses"]:checked'))
                        .map(input => input.value);

                    fetch(`/api/Student/selectCourses/${studentId}`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ courseIds: selectedCourses })
                    })
                        .then(response => {
                            if (!response.ok) {
                                throw new Error("Ders seçimi sırasında bir hata oluştu.");
                            }
                            return response.json();
                        })
                        .then(() => alert('Dersler başarıyla seçildi!'))
                        .catch(error => {
                            console.error('Ders seçimi sırasında hata oluştu:', error);
                            alert('Ders seçimi sırasında bir hata oluştu.');
                        });
                });
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