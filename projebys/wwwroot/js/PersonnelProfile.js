function toggleSubMenu(menuId) {
    const menu = document.getElementById(menuId);
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}

document.addEventListener('DOMContentLoaded', function () {
    let advisorId; // advisorId'yi burada tanımla

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
                advisorId = data.advisorInfo.advisorID; // advisorId'yi al


                // Profil bilgilerini doldur
                populateProfileForm(data.advisorInfo);
            } else {
                alert("Danışman bilgileri eksik.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Danışman bilgileri yüklenirken bir hata oluştu.");
            window.location.href = '/Login/Loginuser';
        });

    // Profil formunu doldur
    function populateProfileForm(profileData) {
        document.getElementById('fullName').value = profileData.fullName;
        document.getElementById('email').value = profileData.email;
        document.getElementById('title').value = profileData.title || 'Başlık Bilgisi Bulunamadı';
    }

    // Profil bilgilerini güncelle
    const profileForm = document.getElementById('profileForm');
    profileForm.addEventListener('submit', function (event) {
        event.preventDefault(); // Sayfa yenilenmesini engelle

        const updatedProfile = {
            fullName: document.getElementById('fullName').value,
            email: document.getElementById('email').value,
            title: document.getElementById('title').value
        };

        fetch(`/api/Advisors/updateProfile/${advisorId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedProfile)
        })
            .then(response => {
                if (!response.ok) throw new Error("Profil güncellenemedi.");
                return response.json();
            })
            .then(data => {
                alert('Profil bilgileri başarıyla güncellendi!');
                window.location.reload();
            })
            .catch(error => {
                console.error("Hata:", error);
                alert('Profil güncelleme işlemi sırasında bir hata oluştu.');
            });
    });
});