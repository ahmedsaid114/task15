(() => {
  const toggle = document.getElementById('menuToggle');
  const nav = document.getElementById('mainNav');
  toggle?.addEventListener('click', () => nav?.classList.toggle('open'));

  const revealItems = document.querySelectorAll('.reveal');
  if ('IntersectionObserver' in window) {
    const observer = new IntersectionObserver((entries, obs) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('is-visible');
          obs.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12 });
    revealItems.forEach(item => observer.observe(item));
  } else {
    revealItems.forEach(item => item.classList.add('is-visible'));
  }

  window.initSeatPicker = function () {
    const map = document.getElementById('seatMap');
    if (!map || map.dataset.ready === 'true') return;
    map.dataset.ready = 'true';
    const rows = ['A','B','C','D','E'];
    const occupied = new Set(['A4','B2','B7','C5','D3','E6']);
    const selected = new Set();
    rows.forEach((row, rowIndex) => {
      for (let number = 1; number <= 8; number++) {
        const code = row + number;
        const seat = document.createElement('button');
        seat.type = 'button'; seat.className = 'seat'; seat.textContent = number;
        seat.setAttribute('aria-label', 'Seat ' + code);
        if (occupied.has(code)) { seat.classList.add('occupied'); seat.disabled = true; }
        seat.addEventListener('click', () => {
          if (selected.has(code)) { selected.delete(code); seat.classList.remove('selected'); }
          else { selected.add(code); seat.classList.add('selected'); }
          update();
        });
        map.appendChild(seat);
      }
    });
    function update() {
      document.getElementById('seatCount').textContent = '(' + selected.size + ' selected)';
      document.getElementById('totalPrice').textContent = 'EGP ' + (selected.size * 150).toLocaleString('en-US');
      document.getElementById('confirmBooking').disabled = selected.size === 0;
      map.querySelectorAll('input[name="seats"]').forEach(input => input.remove());
      selected.forEach(code => {
        const input = document.createElement('input'); input.type = 'hidden'; input.name = 'seats'; input.value = code;
        document.querySelector('.booking-panel').appendChild(input);
      });
    }
  };
  window.initSeatPicker();
})();
