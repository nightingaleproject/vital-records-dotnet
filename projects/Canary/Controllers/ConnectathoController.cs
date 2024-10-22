using BFDR;
using Microsoft.AspNetCore.Mvc;
using VRDR;

namespace canary.Controllers
{
    [ApiController]
    public class ConnectathonController : ControllerBase
    {
        
        /// <summary>
        /// Returns all Connectathon Death Record test messages.
        /// GET /connectathon/vrdr
        /// </summary>
        [HttpGet("Connectathon/vrdr")]
        [HttpGet("Connectathon/vrdr/Index")]
        public DeathRecord[] IndexVRDR()
        {
            return VRDR.Connectathon.Records;
        }

        /// <summary>
        /// Returns all Connectathon Birth Record test messages.
        /// GET /connectathon/bfdr
        /// </summary>
        [HttpGet("Connectathon/bfdr-birth")]
        [HttpGet("Connectathon/bfdr-birth/Index")]
        public BirthRecord[] IndexBFDR()
        {
            return BFDR.Connectathon.Records;
        }
    }
}