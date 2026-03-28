using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class BaseController : ControllerBase
{
    protected readonly ILogger Logger;

    public BaseController(ILogger logger)
    {
        Logger = logger;
    }
}
