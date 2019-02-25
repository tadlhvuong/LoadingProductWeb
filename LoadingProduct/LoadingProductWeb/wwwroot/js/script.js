var isMobile=false;
if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) ) {
    isMobile = true;
}

if(isMobile == true){
    $("body").addClass("ismobile");
    $("#pc-version").empty();

}else{
    $("#mobile-version").empty();
}

$(document).ready(function(){
  $(".img" + 0).css("z-index",10);
  IntitContent();
$(document).on("click",'.pagination li',function(){

  $('html, body').animate({
           scrollTop: $("#breadcrumb").offset().top
         }, 1000, function() {
  });
});
  $('.owl-carousel').owlCarousel({
    loop:true,
    items:1,
    nav:true,
    autoplay:true,
    autoplayTimeout:5000,
    autoplayHoverPause:true
});

$('.multiple-items').slick({
  infinite: true,
  slidesToShow: 3,
  slidesToScroll: 1
});

$('.multiple-items-mobile').slick({
  dots: true,
   infinite: false,
   speed: 300,
   slidesToShow: 3,
   slidesToScroll: 1,
   responsive: [
     {
       breakpoint: 1024,
       settings: {
         slidesToShow: 3,
         slidesToScroll: 3,
         infinite: true,
         dots: true
       }
     },
     {
       breakpoint: 480,
       settings: {
         slidesToShow: 2,
         slidesToScroll: 2
       }
     }
     // You can unslick at a given breakpoint now by adding:
     // settings: "unslick"
     // instead of a settings object
   ]
});


$(document).on("click",".list_slide_menu li",function(){
  changeContent($(this))
});

$(document).on("click",".icon_menu",function(){
  if($(".list_menu_mobile").css("display") == 'none')
    $(".list_menu_mobile").css("display","block");
    else{
        $(".list_menu_mobile").css("display","none");
    }
});
function changeicon(curIndex,preIndex)
{
  $('#slide'+curIndex).addClass('active1').removeClass('default1');
  $('#slide'+preIndex).addClass("default1").removeClass("active1")
}



var indexContentPage2 = 0;

var indexContentPage2pre = 0;

function changeContent(element){
          if(element.hasClass('active'))
                 return;
                 indexContentPage2 = element.attr('data-index');
                 $(".img" + indexContentPage2)
                .addClass("fadeInRight").css("z-index", 10)
                .siblings()
                .removeClass("fadeInRight").css("z-index", 5);
                $(".img" + indexContentPage2)
                  .removeClass("fadeOut")
                  .siblings()
                  .addClass("fadeOut");
                changeicon(indexContentPage2,indexContentPage2pre);

                 indexContentPage2pre = indexContentPage2;

    //console.log(element.getAttribute('data-index'));
  }
});
function IntitContent() {
    var winWidth = window.innerWidth;
    var winHeight = window.innerHeight;
    var scaleWidth = winWidth / 2000;
    var scaleHeight = winHeight / 1000;
    $(".item img.video_content_pc").css("height", window.innerHeight-80);
  }
