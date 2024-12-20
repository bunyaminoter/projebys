// Global değişken: studentID
let studentID = null;

/**
 * Öğrenci bilgilerini API'den al ve studentID'yi global olarak sakla.
 */
function fetchStudentInfo() {
    return fetch('/api/Student/getStudentInfo', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Öğrenci bilgileri alınamadı.");
            }
            return response.json();
        })
        .then(data => {
            if (data && data.studentInfo) {
                studentID = data.studentInfo.studentId; // Global değişkene atama
                console.log(`Öğrenci ID başarıyla alındı: ${studentID}`);
                return studentID;
            } else {
                throw new Error("Öğrenci bilgileri eksik veya hatalı.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Öğrenci bilgileri yüklenirken bir hata oluştu. Giriş sayfasına yönlendiriliyorsunuz.");
            window.location.href = '/Login/Loginuser';
        });
}

/**
 * Dinamik linkleri studentID ile oluştur.
 */
function createStudentLinks() {
    if (!studentID) {
        console.error("Öğrenci ID henüz yüklenmedi. Linkler oluşturulamadı.");
        return;
    }

    // Öğrenciye ait dinamik linkler
    document.getElementById('updateProfileLink').href = `/students/updateProfile/${studentID}`;
    document.getElementById('viewTranscriptLink').href = `/students/transcript/${studentID}`;
    document.getElementById('selectCoursesLink').href = `/students/courses/${studentID}`;
    document.getElementById('myCoursesLink').href = `/students/MyCourses/${studentID}`;

    console.log("Öğrenciye ait dinamik linkler başarıyla oluşturuldu.");
}

/**
 * Logout butonunu başlatır.
 */
function initializeLogoutButton() {
    const logoutButton = document.getElementById('logoutButton');
    if (logoutButton) {
        logoutButton.addEventListener('click', handleLogout);
    }
}

/**
 * Logout işlemini gerçekleştirir.
 */
function handleLogout() {
    fetch('/api/Login/Logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Çıkış işlemi başarısız.");
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                alert(data.message);
                // Giriş ekranına yönlendir
                window.location.href = '/Login/Loginuser';
            } else {
                alert("Çıkış sırasında bir hata oluştu.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Çıkış işlemi sırasında bir hata oluştu.");
        });
}

/**
 * Sayfa yüklendiğinde çalıştırılacak ana fonksiyon.
 */
function initializeStudentLinks() {
    // Öğrenci bilgilerini çektikten sonra dinamik linkleri oluştur
    fetchStudentInfo().then(() => {
        createStudentLinks(); // Öğrenci linklerini oluştur
        initializeLogoutButton(); // Logout butonunu başlat
    });
}

// Sayfa yüklendiğinde başlat
document.addEventListener('DOMContentLoaded', initializeStudentLinks);
