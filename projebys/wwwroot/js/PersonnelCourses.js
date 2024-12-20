function toggleSubMenu(menuId) {
    const menu = document.getElementById(menuId);
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}

document.addEventListener('DOMContentLoaded', function () {
    // Ders listesini yükle
    loadCourses();
});

function loadCourses() {
    fetch('/api/Course/GetAllCourses', {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => response.json())
        .then(data => {
            const coursesTable = document.getElementById('coursesTable').getElementsByTagName('tbody')[0];
            const noCoursesMessage = document.getElementById('noCoursesMessage');

            if (data && data.courses && data.courses.length > 0) {
                data.courses.forEach(course => {
                    const row = createCourseRow(course);
                    coursesTable.appendChild(row);
                });
                noCoursesMessage.style.display = 'none';
            } else {
                noCoursesMessage.style.display = 'block';
            }

        })
        .catch(error => {
            console.error("Hata:", error);
            // alert("Dersler yüklenirken bir hata oluştu.");
        });
}

function createCourseRow(course) {
    const row = document.createElement('tr');

    const courseName = document.createElement('td');
    courseName.textContent = course.courseName;
    row.appendChild(courseName);

    const department = document.createElement('td');
    department.textContent = course.department || 'Bölüm Bilgisi Bulunamadı';
    row.appendChild(department);

    const isMandatory = document.createElement('td');
    isMandatory.textContent = course.isMandatory ? 'Zorunlu' : 'Seçmeli';
    row.appendChild(isMandatory);

    const courseCode = document.createElement('td');
    courseCode.textContent = course.courseCode;
    row.appendChild(courseCode);

    const credit = document.createElement('td');
    credit.textContent = course.credit;
    row.appendChild(credit);

    // İşlemler butonları
    const actions = document.createElement('td');



    const deleteButton = document.createElement('button');
    deleteButton.textContent = 'Sil';
    deleteButton.onclick = function () {
        if (confirm(`Emin misiniz? ${course.courseName} dersini silmek istiyor musunuz?`)) {
            deleteCourse(course.courseID); // Ders silme işlemi
        }
    };

    actions.appendChild(deleteButton);
    row.appendChild(actions);

    return row;
}

function deleteCourse(courseID) {
    fetch(`/api/Course/Delete/${courseID}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert("Ders başarıyla silindi!");
                // Sayfayı tamamen yenileyin
                location.reload();  // Sayfa yenilenecek
            } else {
                alert("Ders silinemedi: " + data.message);
            }
        })
        .catch(error => {
            console.error('Silme hatası:', error);
            alert("Ders silme sırasında bir hata oluştu.");
        });
}
