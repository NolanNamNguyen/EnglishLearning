const heightWin = window.screen.height;
	const listA = document.querySelectorAll('.header__nav a');
	const banner = document.querySelector('.banner');
	const start = document.querySelector('.start-video');
	const close = document.querySelector('.close');
	const video = document.querySelector('.video-story');
	const text = document.querySelector('.banner__infor h1');
	banner.style.height = heightWin + 'px';
	start.addEventListener('click',function(){
		video.classList.add('dis');
		text.classList.add('opa');
	});
	close.addEventListener('click',function(){
		video.classList.remove('dis');
		text.classList.remove('opa');
	});

	listA.forEach(function(btn){
		btn.addEventListener('click',function(e){
			e.preventDefault();
			let data = this.getAttribute('data');
			let index = document.querySelector('.' + data).offsetTop;
			window.scrollTo({
				top:index,
				behavior: 'smooth'
			});
		});
	});




	let i,lengthTab,lengthListTab;
	const listBtn = document.querySelectorAll('.tab span');
	lengthTab = listBtn.length;
	const listForm = document.querySelectorAll('.form form');

	const listTab = document.querySelectorAll('.banner__nav a');
	lengthListTab = listTab.length;
	const listBox = document.querySelectorAll('.banner__wrapper-box');
	console.log(listBox);

	listBtn.forEach(function(btn){
		btn.onclick = function()
		{
			
			for(i = 0;i < lengthTab ;i++)
			{
				listBtn[i].classList.remove('active');
				listForm[i].classList.remove('active');
			}
			this.classList.add('active');
			let classBox = this.getAttribute('data-class');
			document.querySelector('.' + classBox).classList.add('active');
		}
	});

	listTab.forEach(function(btn){
		btn.onclick = function(e)
		{
			e.preventDefault();

			for(i = 0;i < lengthListTab ;i++)
			{
				listTab[i].classList.remove('active');
				listBox[i].classList.remove('active');
			}
			this.classList.add('active');
			let classTab = this.getAttribute('data-class');
			document.querySelector('.' + classTab).classList.add('active');
		}
	});

	const go = document.querySelector('.letgo');
	const formLog = document.querySelector('.formLog');
	const close_form = document.querySelector('.close__form');
	go.addEventListener('click',e => {
		e.preventDefault();
		window.scrollTo({
			top : 0,
			behavior: 'smooth'
		});
		formLog.classList.add('ht');
	})
	close_form.addEventListener('click',(e) => {
		formLog.classList.remove('ht');
	});