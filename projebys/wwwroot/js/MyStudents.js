function toggleSubMenu(menuId) {
    const menu = document.getElementById(menuId);
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}

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
                const advisorId = data.advisorInfo.advisorID;

                // Öğrenci listesini yükle
                loadStudents(advisorId);
            } else {
                alert("Danışman bilgileri eksik.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Kullanıcı bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser';
        });

    function loadStudents(advisorId) {
        fetch(`/api/Advisors/GetMyStudents/${advisorId}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        })
            .then(response => response.json())
            .then(data => {
                const studentsTable = document.getElementById('studentsTable').getElementsByTagName('tbody')[0];
                const noStudentsMessage = document.getElementById('noStudentsMessage');

                if (data && data.students && data.students.length > 0) {
                    data.students.forEach(student => {
                        const row = createStudentRow(student);
                        studentsTable.appendChild(row);
                    });
                    noStudentsMessage.style.display = 'none';
                } else {
                    noStudentsMessage.style.display = 'block';
                }
            })
            .catch(error => {
                console.error("Hata:", error);
                //alert("Öğrenciler yüklenirken bir hata oluştu.");
            });
    }

    function createStudentRow(student) {
        const row = document.createElement('tr');

        const studentName = document.createElement('td');
        studentName.textContent = student.fullName;
        row.appendChild(studentName);

        const studentID = document.createElement('td');
        studentID.textContent = student.studentID;
        row.appendChild(studentID);

        const department = document.createElement('td');
        department.textContent = student.department || 'Bölüm Bilgisi Bulunamadı';
        row.appendChild(department);

        const email = document.createElement('td');
        email.textContent = student.email;
        row.appendChild(email);
        return row;
    }
});