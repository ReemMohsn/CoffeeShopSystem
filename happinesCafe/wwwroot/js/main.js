
document.addEventListener('DOMContentLoaded', function () {
    // mainpage ************************************************************
    let navbar = document.querySelector('.navbar');
    let header = document.querySelector('header');
    document.querySelector('#menu-btn').onclick = () => {   //لما نضغط على الليقونة حق القائمة المنبتقة يا إما تفتح القائمة أو تقفل 
        navbar.classList.toggle('active');
    }

    window.onscroll = () => {
        navbar.classList.remove('active');  //طريقة ثانية لقفل القائمة المنبتقهوهي من خلال عمل سكرول على الصفحة لتحت أو لفوق
        header.classList.toggle('shadow', window.scrollY > 0)

    }

    // document.addEventListener('DOMContentLoaded', () => {
    //  عندما يإتي الماوس sign ألوان زر ال
    let sign = document.querySelector('.sign');
    let signlogo = document.querySelector('.signLogo');
    sign.onmouseenter = () => {
        signlogo.src = '/imges/3.png';
    }
    sign.onmouseleave = () => {
        signlogo.src = '/imges/4.png';


    }



    // تغيير محتوى شكل القلب عندما يأتي المؤشر عليه
    let galeb = document.querySelector('.galeb');
    galeb.onmouseenter = () => {
        galeb.src = '/imges/galeb.png';
    }
    galeb.onmouseleave = () => {
        galeb.src = '/imges/123.png';
    }


    //  عندما يإتي الماوس عليهExploreعكس ألوان زر ال
    let Explore = document.querySelector('.Explore');
    let sahem1 = document.querySelector('.sahem1');
    if (Explore) {
        Explore.onmouseenter = () => {
            sahem1.src = "/imges/63.png";
        }
        Explore.onmouseleave = () => {
            sahem1.src = "/imges/47.png";
        }
    }



    //  scroll 
    const MainPge = document.querySelector('.MainPge');
    if (MainPge) {
        const sr = ScrollReveal({
            origin: 'top',
            distance: '40px',
            duration: 1500,
            reset: true
        });
        sr.reveal('.row,.ProductsContener,.bigtital,.describtion-bigtital,.Order-Now,.feturr,.SubHeader,.tiata,.item1,.item2 ,.part1,.sahem2,.coffeebag,.logo2,.line,.socalMedia,.navbardowen,.boxcontainer');
    }


    //// google
    //function openpopup1() {
    //    var interface=document.getElementById("myinterface1");
    //    var popup=document.getElementById("mypopup1");
    //    interface.style.opacity=".3"
    //    popup.style.display="block"
    //}
    //function closepopup1() {
    //    var interface=document.getElementById("myinterface1");
    //    var popup=document.getElementById("mypopup1");
    //    interface.style.opacity="1"
    //    popup.style.display="none"
    //}



    // Check Of Contactus page 
    let FormContactus = document.getElementById("FormContactus");
    if (FormContactus) {
        FormContactus.onsubmit = function () {
            let phoneId = document.getElementById("number").value;
            let email = document.getElementById("email").value;
            let Name = document.getElementById("Name").value;
            let nameRith = /^[a-zA-Z]+$/;
            let numberRith = /\d{9}/;
            let emailRith = /\w+@\w+.(com|net)/;
            let result = numberRith.test(phoneId);
            let result2 = emailRith.test(email);
            let result3 = nameRith.test(Name);
            if (result3 === false) {
                window.alert("The name should contain crecters just!? ")
                return false;
            } else {
                if (result2 === false) {
                    window.alert("The sentacs of email is not correct!? ")
                    return false;
                } else {
                    if (result === false) {
                        window.alert("The lenght of phone number is not correct !? ")
                        return false;
                    }
                }
            }
            return true;
        }
    }


});







