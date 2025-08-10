async function loadWallpapers() {
  const res = await fetch('wallpapers.json');
  const data = await res.json();
  const categoriesNav = document.getElementById('categories');
  const grid = document.getElementById('wallpaper-grid');

  function renderCategory(category) {
    grid.innerHTML = '';
    category.wallpapers.forEach(wp => {
      const img = document.createElement('img');
      img.src = wp.thumb || wp.image;
      img.alt = wp.title;
      img.addEventListener('click', () => openPreview(wp));
      grid.appendChild(img);
    });
  }

  data.categories.forEach((cat, idx) => {
    const link = document.createElement('a');
    link.textContent = cat.name;
    link.addEventListener('click', () => {
      document.querySelectorAll('nav a').forEach(a => a.classList.remove('active'));
      link.classList.add('active');
      renderCategory(cat);
    });
    if (idx === 0) {
      link.classList.add('active');
      renderCategory(cat);
    }
    categoriesNav.appendChild(link);
  });
}

function openPreview(wp) {
  const preview = document.getElementById('preview');
  const img = document.getElementById('preview-image');
  const download = document.getElementById('download-link');
  img.src = wp.image;
  download.href = wp.image;
  preview.classList.remove('hidden');
}

function closePreview() {
  document.getElementById('preview').classList.add('hidden');
}

loadWallpapers();

document.getElementById('close-preview').addEventListener('click', closePreview);
document.querySelector('#preview .overlay').addEventListener('click', closePreview);
