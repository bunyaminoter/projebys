﻿// Alt menü aç/kapa fonksiyonu
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
            // Data doğru geldiği durumda bilgileri sayfada göster
            if (data) {
                document.getElementById('advisorName').textContent = data.advisorInfo.fullName;
                document.getElementById('advisorTitle').textContent = data.advisorInfo.title || "Başlık Bilgisi Bulunamadı";
                document.getElementById('advisorDepartment').textContent = data.advisorInfo.department || "Bölüm Bilgisi Bulunamadı";
                document.getElementById('advisorEmail').textContent = data.email;


            } else {
                console.error("Danışman bilgileri eksik veya hatalı.");
                alert("Danışman bilgileri yüklenirken bir hata oluştu.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Danışman bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser'; // Kullanıcı giriş ekranına yönlendiriliyor
        });
});
