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
    // Kullanıcı bilgilerini yükle (örneğin advisorId için)
    fetch('/api/Advisors/getAdvisorInfo', {
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
                const advisorId = data.advisorInfo.advisorID;


                // Ders ekleme formu işlemleri
                const courseAddForm = document.getElementById('courseAddForm');
                courseAddForm.addEventListener('submit', function (event) {
                    event.preventDefault(); // Formun varsayılan gönderilmesini engelle

                    const formData = new FormData(courseAddForm);
                    const courseData = {};

                    // FormData içeriğini nesneye dönüştür
                    formData.forEach((value, key) => {
                        // IsMandatory alanını boolean'a dönüştür
                        if (key === "IsMandatory") {
                            courseData[key] = value === "true"; // "true" string'ini boolean'a dönüştür
                        } else {
                            courseData[key] = value;
                        }
                    });

                    console.log("Gönderilen JSON:", courseData); // Kontrol için logla



                    // API'ye advisorId ile POST isteği gönder
                    fetch(`/api/Course/Add`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(courseData)
                    })
                        .then(response => response.json())
                        .then(data => {
                            if (data.success) {
                                alert('Ders başarıyla eklendi!');
                                window.location.href = `/personnel/courses/list`; // Derslerim sayfasına yönlendir
                            } else {
                                alert('Ders ekleme sırasında bir hata oluştu!');
                            }
                        })
                        .catch(error => {
                            console.error('Hata:', error);
                            alert('Ders eklerken bir hata oluştu.');
                        });
                });
            } else {
                alert("Danışman bilgileri eksik.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Kullanıcı bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser'; // Kullanıcı giriş ekranına yönlendirme
        });
});