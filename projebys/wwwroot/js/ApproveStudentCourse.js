function toggleSubMenu(menuId) {
    const menu = document.getElementById(menuId);
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}
let advisorId; // Declare advisorId globally
document.addEventListener('DOMContentLoaded', function () {
    // Danışman bilgilerini yükle
    fetch('/api/Advisors/getAdvisorInfo', {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => {
            if (!response.ok) throw new Error("Danışman bilgileri alınamadı.");
            return response.json();
        })
        .then(data => {
            if (data && data.advisorInfo) {
                advisorId = data.advisorInfo.advisorID;

                // Onay bekleyen dersleri yükle
                loadPendingApprovals(advisorId);
            } else {
                alert("Danışman bilgileri eksik.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Danışman bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser'; // Hata durumunda giriş sayfasına yönlendirme
        });

    function loadPendingApprovals(advisorId) {
        fetch(`/api/Advisors/GetMyStudents/${advisorId}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        })
            .then(response => {
                if (!response.ok) throw new Error("Öğrenci listesi alınamadı.");
                return response.json();
            })
            .then(data => {
                console.log("Öğrenci Verileri:", data);
                const approvalTable = document.getElementById('approvalTable').getElementsByTagName('tbody')[0];
                const noApprovalsMessage = document.getElementById('noApprovalsMessage');

                if (data.students && data.students.length > 0) {
                    noApprovalsMessage.style.display = 'none';

                    // Her öğrenci için ders bilgilerini almak
                    const promises = data.students.map(student => {
                        if (!student.studentID) {
                            console.error("StudentID eksik:", student);
                            return Promise.resolve(); // Eksik ID için işlemi atla
                        }

                        return fetch(`/api/Advisors/GetStudentCourses/${student.studentID}`, {
                            method: 'GET',
                            headers: { 'Content-Type': 'application/json' }
                        })
                            .then(response => {
                                if (!response.ok) {
                                    console.error(`Ders bilgileri alınamadı (StudentID: ${student.studentID}).`);
                                    return null; // Hata durumunda null döndür
                                }
                                return response.json();
                            })
                            .then(courseData => {
                                if (courseData) {
                                    courseData.forEach(course => {
                                        if (!course.isApproved) { // Sadece onaylanmamış dersler
                                            const row = createApprovalRow(student, course);
                                            approvalTable.appendChild(row);
                                        }
                                    });
                                }
                            })
                            .catch(error => {
                                console.error(`Hata (Dersler - StudentID: ${student.studentID}):`, error);
                            });
                    });

                    // Tüm istekler tamamlandığında
                    Promise.all(promises).then(() => {
                        console.log("Tüm ders verileri başarıyla yüklendi.");
                    });
                } else {
                    noApprovalsMessage.style.display = 'block';
                }
            })
            .catch(error => {
                console.error("Hata (Öğrenciler):", error);
            });
    }

    // Tablo satırı oluştur
    function createApprovalRow(student, course) {
        console.log(student.studentID);  // studentID'yi burada doğru kullanıyoruz
        console.log(course.courseID);  // courseID'yi de doğru kullanıyoruz

        const row = document.createElement('tr');

        const studentName = document.createElement('td');
        studentName.textContent = student.fullName;
        row.appendChild(studentName);

        const studentID = document.createElement('td');
        studentID.textContent = student.studentID;
        row.appendChild(studentID);

        const courseName = document.createElement('td');
        courseName.textContent = course.courseName || 'Ders Bilgisi Bulunamadı';
        row.appendChild(courseName);

        const approveButton = document.createElement('td');
        const button = document.createElement('button');
        button.className = "approveButton";
        button.textContent = "Onayla";

        button.onclick = function () { approveStudentCourse(student.studentID, course.courseID); };
        approveButton.appendChild(button);
        row.appendChild(approveButton);

        return row;
    }

    // Bir öğrencinin dersi onaylama işlemi
    function approveStudentCourse(studentId, courseId) {
        fetch(`/api/Advisors/ApproveStudentCourse/${studentId}/${courseId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
        })
            .then(response => {
                if (response.ok) {
                    alert("Ders başarıyla onaylandı!");
                    // Onaydan sonra tabloyu güncelle
                    document.getElementById('approvalTable').getElementsByTagName('tbody')[0].innerHTML = '';
                    loadPendingApprovals(advisorId);
                } else {
                    response.json().then(data => alert(data.message || "Onay sırasında bir hata oluştu."));
                }
            })
            .catch(error => {
                console.error("Hata:", error);
                alert("Bir hata oluştu.");
            });
    }
});