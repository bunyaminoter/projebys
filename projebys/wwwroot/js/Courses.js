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



                // Öğrencinin derslerini listele
                fetch(`/api/Course/GetCoursesByClass/${studentId}`)
                    .then(response => {
                        if (!response.ok) {
                            throw new Error("Ders listesi alınamadı.");
                        }
                        return response.json();
                    })
                    .then(courses => {
                        console.log("Ders Listesi:", courses); // Gelen veriyi kontrol et

                        const coursesTableBody = document.querySelector('#coursesTable tbody');
                        if (!coursesTableBody) {
                            console.error("Tablonun tbody kısmı bulunamadı.");
                            return;
                        }

                        // Dersleri tabloya ekle
                        courses.courses.forEach(course => {
                            const row = coursesTableBody.insertRow();
                            row.innerHTML = `
                                <td>${course.courseCode || 'Kod Yok'}</td>
                                <td>${course.courseName}</td>
                                <td>${course.credit}</td>
                                <td>${course.isMandatory ? 'Zorunlu' : 'Seçmeli'}</td>
                                <td id="quota-${course.courseID}">Yükleniyor...</td>
                                <td><input type="checkbox" name="SelectedCourses" value="${course.courseID}"></td>
                            `;
                        });

                        // Kontenjan bilgilerini al
                        courses.courses.forEach(course => {
                            fetch(`/api/Course/getCourseQuota/${course.courseID}`)
                                .then(response => response.json())
                                .then(quotaData => {
                                    const quotaElement = document.getElementById(`quota-${course.courseID}`);
                                    quotaElement.textContent = `${quotaData.quota - quotaData.remainingQuota} / ${quotaData.quota}`;
                                });
                        });
                    })
                    .catch(error => console.error('Ders listesi yüklenirken hata oluştu:', error));

                // Ders seçme formunu işleme
                document.getElementById('courseSelectionForm').addEventListener('submit', function (event) {
                    event.preventDefault();

                    // Seçilen derslerin ID'lerini topla
                    const SelectedCourses = Array.from(document.querySelectorAll('input[name="SelectedCourses"]:checked'))
                        .map(input => parseInt(input.value, 10));

                    if (SelectedCourses.length === 0) {
                        alert("Lütfen en az bir ders seçin.");
                        return;
                    }

                    // API'ye gönderilecek veri
                    const data = { courseID: SelectedCourses };
                    console.log(data);

                    // Ders seçimlerini API'ye gönder
                    fetch(`/api/Student/selectCourses/${studentId}`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(SelectedCourses) // Düz dizi olarak gönder
                    })
                        .then(response => {
                            if (!response.ok) {
                                throw new Error("Ders seçimi sırasında bir hata oluştu.");
                            }
                            return response.json();
                        })
                        .then(() => alert('Dersler başarıyla kaydedildi!'))
                        .catch(error => {
                            console.error('Ders seçimi sırasında hata oluştu:', error);
                            alert('Ders seçimi sırasında bir hata oluştu:  Daha önceden ders seçimi yaptınız...');
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
