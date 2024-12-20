        // Alt menü aç/kapa fonksiyonu
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

                    // Transkript verilerini API'den al ve tabloyu doldur
                    fetch(`/api/Student/transcript/${studentId}`)
                        .then(response => response.json())
                        .then(transcriptData => {
                            if (transcriptData && transcriptData.length > 0) {
                                // Transkript tablosunu dolduruyoruz
                                const transcriptTable = document.getElementById('transcriptTable').getElementsByTagName('tbody')[0];
                                transcriptTable.innerHTML = ''; // Eski satırları temizle

                                transcriptData.forEach(course => {
                                    const row = transcriptTable.insertRow();
                                    row.innerHTML = `
                                        <td>${course.courseName || 'Ders Bilgisi Bulunamadı'}</td>
                                        <td>${course.credit || '0'}</td>
                                        <td>${course.grade || 'Not Yok'}</td>
                                    `;
                                });
                            } else {
                                console.error("Transkript bilgisi eksik veya hatalı.");
                                alert("Transkript bilgileri yüklenirken bir hata oluştu.");
                            }
                        })
                        .catch(error => {
                            console.error("Hata:", error);
                            alert("Transkript bilgileri yüklenirken bir hata oluştu.");
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