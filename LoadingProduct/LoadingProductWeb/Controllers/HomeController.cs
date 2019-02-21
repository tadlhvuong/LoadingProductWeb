using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoadingProductWeb.Models;

namespace LoadingProductWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Portfolio()
        {
            ViewData["Message"] = "Your portfolio page.";

            return View();
        }

        public IActionResult Service()
        {
            ViewData["Message"] = "Your service page.";

            return View();
        }

        public IActionResult DetailsService(int id)
        {
            Service service1 = new Service(1,
                                          "Bốc xếp hàng hóa",
                                          "Dịch vụ bốc xếp hàng hóa Công ty chúng tôi tiếp nhận cuộc gọi dịch vụ và chăm sóc khách hàng 24/24, làm việc 24/24 theo khung giờ quý khách hàng chọn lựa.",
                                         "Nhu cầu sử dụng dịch vụ bốc xếp tại TP.Hồ Chí Minh,Bình Dương đang ngày càng phát triển. Đặc biệt là khi phải vận chuyển hàng hóa cồng kềnh ở những nơi chật hẹp, nhiều ngõ hẻm, dân cư đông đúc rất khó cho các phương tiện máy móc hoạt động. Hiểu được những khó khăn và lo lắng đó, Dịch Vụ Bốc Xếp chúng tôi xin cung cấp đến Quý khách hàng dịch vụ bốc xếp hàng hóa TP.HCM,Bình Dương nhanh chóng, an toàn và tiết kiệm nhất.",
                                          "/img/icons/bocxep.jpg");
            Service service2 = new Service(2,
                                        "Chuyển nhà & Văn phòng",
                                        "Dịch vụ chuyển nhà chúng tôi luôn mang đến cho quý khách hài lòng, và từng ngày xây dựng được lòng tin tưởng với khách hàng. Không chỉ cam kết chất lượng dịch vụ hàng đầu về sự chuyên nghiệp, uy tín mà chúng tôi còn có giá dịch vụ tốt nhất.",
                                       "Là một trong những dịch vụ chuyển nhà uy tín, tiện lợi nhất khu vực Sài Gòn, Bình Dương. Khi quý khách hàng có nhu cầu vận chuyển, chuyển nhà chỉ cần gọi đến công ty chúng tôi, tất cả những công việc còn lại sẽ được nhân viên của chúng tôi thực hiện, từ việc: tháo lắp các thiết bị – đồ dùng , đóng gói vào thùng carton (nguyên vật liệu cho việc đóng gói sẽ được miễn phí), cho đến sắp xếp vận chuyển lên xe tải chuyên dụng, rồi đem đến địa điểm mới, sắp xếp và chuyển đến đúng nơi theo đúng yêu cầu của quý khách hàng. <\br> Ngoài ra còn dịch vụ khác như chuyển văn phòng, chuyển kho xưởng, dọn dẹp kho bãi tại Sài Gòn Bình Dương.",
                                        "/img/icons/chuyenvp.png");
            Service service3 = new Service(3,
                                        "Cho thuê nhân công",
                                        "Dịch vụ Cho thuê nhân công bốc xếp tại Tphcm,Bình Dương,quý khách có nhu cầu chuyển kho xưởng hoặc thuê nhân công bốc xếp kho chuyên nghiệp.Hãy đến với chúng tôi,công ty chúng tôi sẽ nhanh chóng cử công nhân , đến tận nơi.",
                                       "Dịch vụ cho thuê nhân công của công ty Bốc Xếp chúng tôi được sử dụng để đáp ứng các đơn hàng ngắn hạn, các dự án ngắn hạn, các vị trí cần thay thế trong một khoảng thời gian ngắn, hoặc thay thế tạm thời ở một vị trí khuyết nào đó của quý công ty.",
                                        "/img/icons/thuenhancong.jpg");
            Service service4 = new Service(4,
                                        "Cho thuê xe nâng cẩu",
                                        "Dịch vụ cho thuê xe nâng cẩu Công ty chúng tôi luôn cung cấp các lọa thiết bị máy móc nhằm phục vụ khách hàng một cách hiệu quả nhất.",
                                       "Công ty chúng tôi có đội xe cẩu – xe nâng cực kỳ hùng mạnh, luôn đáp ứng nhu cầu cho khách hàng khắp nơi trên các khu vực Bình Dương , Tphcm hay các khu vực lân cận. Hãy liên hệ ngay để được phục vụ tận nơi..",
                                        "/img/icons/thuexe.png");

            Service[] services = { service1, service2, service3, service4 };

            ViewData["DetailService"] = services[id - 1];

            ViewData["Message"] = "Your service page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
