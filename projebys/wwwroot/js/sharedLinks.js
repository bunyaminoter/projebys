// Global değişken: advisorID
let advisorID = null;

/**
 * Danışman bilgilerini API'den al ve advisorID'yi global olarak sakla.
 */
function fetchAdvisorInfo() {
    return fetch('/api/Advisors/getAdvisorInfo', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Danışman bilgileri alınamadı.");
            }
            return response.json();
        })
        .then(data => {
            if (data && data.advisorInfo) {
                advisorID = data.advisorInfo.advisorID; // Global değişkene atama
                console.log(`Advisor ID başarıyla alındı: ${advisorID}`);
                return advisorID;
            } else {
                throw new Error("Danışman bilgileri eksik veya hatalı.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Danışman bilgileri yüklenirken bir hata oluştu. Giriş sayfasına yönlendiriliyorsunuz.");
            window.location.href = '/Login/Loginuser';
        });
}

/**
 * Dinamik linkleri advisorID ile oluştur.
 */
function createDynamicLinks() {
    if (!advisorID) {
        console.error("Advisor ID henüz yüklenmedi. Linkler oluşturulamadı.");
        return;
    }

    const links = {
        updateProfileLink: `/Personnel/Profile/${advisorID}`,
        myStudentsLink: `/Personnel/MyStudents/${advisorID}`,
        addCourseLink: `/personnel/courses/add/${advisorID}`,
        listCourseLink: `/personnel/courses/list`,
        approveStudentCourseLink: `/personnel/approveStudentCourse/${advisorID}`
    };

    for (const [id, url] of Object.entries(links)) {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener('click', function (event) {
                event.preventDefault();
                window.location.href = url;
            });
        }
    }

    console.log("Dinamik linkler başarıyla oluşturuldu:", links);
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
function initializeSharedLinks() {
    fetchAdvisorInfo().then(() => {
        createDynamicLinks(); // Dinamik linkleri oluştur
        initializeLogoutButton(); // Logout butonunu başlat
    });
}

// Sayfa yüklendiğinde başlat
document.addEventListener('DOMContentLoaded', initializeSharedLinks);
