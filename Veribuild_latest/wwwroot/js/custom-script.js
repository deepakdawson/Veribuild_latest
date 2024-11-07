

const optionMenu = document.querySelector(".btn-nav"),
  selectBtn = optionMenu.querySelector(".nav-link"),
  options = optionMenu.querySelectorAll(".option"),
  sBtn_text = optionMenu.querySelector(".sBtn-text");

selectBtn.addEventListener("click", () =>
  optionMenu.classList.toggle("active")
);




//const dropImages = document.querySelectorAll('.drop-image');
//for(let i=0; i< dropImages.length; i++){
//	dropImages[i].addEventListener('change', function(){
//	var curElement = dropImages[i].parentElement.querySelector('img');
//	var reader = new FileReader();
//	reader.onload = function (e) {
//		curElement.setAttribute("src", e.target.result);
//		curElement = dropImages[i].parentElement.classList.add('border');
//	};
//	reader.readAsDataURL(this.files[0]);
//	});
//}




//const dropFiles = document.querySelectorAll('.drop-file');
//for(let i=0; i< dropFiles.length; i++){
//	dropFiles[i].addEventListener('change', function(e){
	
//	var canvasElement = dropFiles[i].parentElement.querySelector("canvas")
//	var file = e.target.files[0]
//	if(file.type != "application/pdf"){
//		console.error(file.name, "is not a pdf file.")
//		return
//	}
	
//	var fileReader = new FileReader();  

//	fileReader.onload = async (result) => {
//		var typeArray = new Uint8Array(result.target.result);
//		const pdf = await PDFJS.getDocument(typeArray)
//		const page = await pdf.getPage(pdf.numPages)
//		var viewport = page.getViewport(2.0);
//		var canvas = dropFiles[i].parentElement.querySelector("canvas");
//		canvasElement = dropFiles[i].parentElement.classList.add('border');
//		canvas.height = viewport.height;
//		canvas.width = viewport.width;

//		page.render({
//			canvasContext: canvas.getContext('2d'),
//			viewport: viewport
//		});
//	};

//	fileReader.readAsArrayBuffer(file);
	
//	});
//}

//const selectImage = document.querySelectorAll('.image-gallery #propertyImage img');
//for(let i=0; i< selectImage.length; i++){
//	selectImage[i].addEventListener('click', function(e){
//    document.getElementById("main_img").src = e.target.src;
//	});
//}	

//  const video = document.getElementById("video");
//  const circlePlayButton = document.getElementById("circle-play-b");
  
//  if(circlePlayButton){
//	 circlePlayButton.addEventListener("click", togglePlay);
//	  video.addEventListener("playing", function() {
//		circlePlayButton.style.opacity = 0;
//	  });
//	  video.addEventListener("pause", function() {
//		circlePlayButton.style.opacity = 1;
//	  });

//	  function togglePlay() {
//		if(video.paused || video.ended) {
//		  video.play();
//		} else {
//		  video.pause();
//		}
//	  } 
//  }
  
  
//  var commonSelect = document.getElementById('checkMain');
//  if(commonSelect){
//	commonSelect.addEventListener('change', function () {
//    let checkboxes =  document.querySelectorAll('.select-input-box .d-flex figure .form-check:first-of-type .form-check-input');
//    checkboxes.forEach(function (checkbox) {
//    checkbox.checked = this.checked;
//	}, this);
//  });  
//  }
    
  
  
  
//  $(".other-trade .flip").click(function() {
//    $(this).toggleClass("fw-bold");
//    $(this).find("i").toggleClass("fa-solid fa-chevron-down fa-solid fa-chevron-up");
//    $(this).parent().find(".panel").slideToggle();
//  });
  
  
//   $("#menu-toggle").click(function(e) {
//    e.preventDefault();
//	var isIE11 = !!navigator.userAgent.match(/Trident.*rv\:11\./);
//        $("#wrapper").toggleClass("toggled");

//  	if(isIE11){
//        if($("#wrapper").hasClass("toggled")){
//	    $('#sidebar-wrapper').css("margin-left", "-268px")
//	} 
//	else {
//	    $('#sidebar-wrapper').css("margin-left", "-250px")	
//    }	 
//	}
//   });
   
   
   
//   	$(document).ready(function() {
//		$('.minus').click(function () {
//			var $input = $(this).parent().find('input');
//			var count = parseInt($input.val()) - 1;
//			count = count < 1 ? 1 : count;
//			$input.val(count);
//			$input.change();
//			return false;
//		});
//		$('.plus').click(function () {
//			var $input = $(this).parent().find('input');
//			$input.val(parseInt($input.val()) + 1);
//			$input.change();
//			return false;
//		});
//		$('#datepicker').datepicker({}).datepicker('update', new Date());
//	});
	
	
//	$('.image-tradesman').change(function(){
//		var curElement = $('.myDetail-box .image');
//		var reader = new FileReader();
//		reader.onload = function (e) {
//			curElement.attr('src', e.target.result);
//		};
//		reader.readAsDataURL(this.files[0]);
//	});
	
	//var addPDF = document.getElementById('add-pdf-box');
	//  if(addPDF){
	//	addPDF.addEventListener('click', function () {
	//	var customEle = '<div class="col-sm-2 position-relative"><span class="btnClose" onclick="removeUrl(this)"><i class="fa-solid fa-xmark"></i></span><div class="form-group"><h5 class="file-upload-head">Credentials</h5><div class="file-wrapper"><label class="w-100" for="doc113"><img src="assets/img/pdf-demo.svg" class="img-fluid" /><input class="drop-file" type="file" id="doc113"></label><div class="title-field"><input type="text" class="form-control" placeholder="title" /></div></div></div></div>';
	//	addPDF.parentElement.parentElement.parentElement.insertAdjacentHTML('beforeend', customEle)
	//  });  
 //   }
	
//	$(function () {
//      $('#mobile1').intlTelInput({
//        autoHideDialCode: true,
//        formatOnDisplay: true,
//        initialCountry: "us",
//        nationalMode: false,
//        separateDialCode: true
//      });
//	  $('#no-limit .select2').select2({
//		multiple: "multiple",
//	  });
//    });
	
//	function dynamicUrl(item){
//		var inputDiv = '<div class="mt-3 position-relative"><span class="btnClose" onclick="removeUrl(this)"><i class="fa-solid fa-xmark"></i></span><input type="text" class="form-control" placeholder="Enter vimeo URL" /></div>';
//		document.querySelector('#'+item).insertAdjacentHTML('beforeend', inputDiv);
//	}
	
//	function removeUrl(item){
//		item.parentElement.remove();
//	}
	
	


  


