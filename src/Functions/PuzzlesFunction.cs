using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using HGV.Warhorn.Api.Data;
using HGV.Warhorn.Api.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HGV.Warhorn.Api.Functions
{
    public class PuzzlesFunction
    {
        private readonly PuzzleContext context;
        private readonly IMapper mapper;

        public PuzzlesFunction(PuzzleContext cxt, IMapper mapper)
        {
            this.context = cxt;
            this.mapper = mapper;
        }

        [FunctionName("Authorize")]
         public async Task<IActionResult> Authorize(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authorize")] HttpRequest req,
            ILogger log)
        {
            var claimed_id = req.Query["openid.claimed_id"].ToString();
            var identity = req.Query["openid.identity"].ToString();
            var sig = req.Query["openid.sig"].ToString();
            var token = "";
            return new OkObjectResult(token);
        }

        [FunctionName("NewPuzzles")]
         public async Task<IActionResult> New(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "puzzles/new")] HttpRequest req,
            ILogger log)
        {
            // return new ChallengeResult("api");

            var limit = DateTime.UtcNow.AddDays(-7);
            var query = context.Puzzles.Where(_ => _.CreatedWhen > limit);
            var collection = await mapper.ProjectTo<PuzzleDTO>(query).ToListAsync();
            return new OkObjectResult(collection);
        }

        [FunctionName("HotPuzzles")]
         public async Task<IActionResult> Hot(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "puzzles/hot")] HttpRequest req,
            ILogger log)
        {
            var limit = DateTime.UtcNow.AddDays(-7);
            var query = context.Puzzles.Where(_ => _.CreatedWhen > limit).OrderByDescending(_ => _.UpVotes);
            var collection = await mapper.ProjectTo<PuzzleDTO>(query).ToListAsync();
            return new OkObjectResult(collection);
        }

        [FunctionName("PopularPuzzles")]
         public async Task<IActionResult> Popular(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "puzzles/popular")] HttpRequest req,
            ILogger log)
        {
            var query = context.Puzzles.OrderByDescending(_ => _.UpVotes);
            var collection = await mapper.ProjectTo<PuzzleDTO>(query).ToListAsync();
            return new OkObjectResult(collection);
        }

        [FunctionName("MyPuzzles")]
         public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "puzzles/mine")] HttpRequest req,
            ILogger log)
        {
            var userId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
            if(string.IsNullOrWhiteSpace(userId))
                return new UnauthorizedResult();

            var query = context.Puzzles.Where(_ => _.UserId == userId);
            var collection = await mapper.ProjectTo<PuzzleDTO>(query).ToListAsync();
            return new OkObjectResult(collection);
            
        }

         [FunctionName("GetPuzzle")]
         public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "puzzle/get/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            var entity = await context.Puzzles.Where(_ => _.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                return new NotFoundResult();

            var dto = this.mapper.Map<PuzzleDTO>(entity);
            return new OkObjectResult(dto);
        }

        [FunctionName("SavePuzzle")]
        public async Task<IActionResult> Save(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "puzzle/save")]HttpRequest req,
            ILogger log)
        {
            var userId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
            if(string.IsNullOrWhiteSpace(userId))
                return new UnauthorizedResult();

            var inputDto = await req.GetBodyAsync<PuzzleDTO>();

            var entity = this.context.Puzzles.Persist(this.mapper).InsertOrUpdate<PuzzleDTO>(inputDto);
            if(entity.UserId == null)
            {
                entity.UserId = userId;
                entity.CreatedWhen = DateTime.UtcNow;
            }
            else if(entity.UserId != userId)
            {
                return new UnauthorizedResult();
            }

            entity.LastUpdate = DateTime.UtcNow;

            await this.context.SaveChangesAsync();

            var outputDto = this.mapper.Map<PuzzleDTO>(entity);
            return new OkObjectResult(outputDto);
        }

        [Authorize]
        [FunctionName("VoteForPuzzle")]
         public async Task<IActionResult> Vote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "puzzle/vote")] HttpRequest req,
            ILogger log)
        {
            var userId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
            if(string.IsNullOrWhiteSpace(userId))
                return new UnauthorizedResult();

            var dto = await req.GetBodyAsync<PuzzleVoteDTO>();
            var entity = this.context.Votes.Persist(this.mapper).InsertOrUpdate<PuzzleVoteDTO>(dto);
            entity.UserId = userId;
            entity.Date = DateTime.UtcNow;

            await this.context.SaveChangesAsync();

            return new OkResult();
        }

        /*
        {
            const returnUrl = encodeURIComponent(window.location.origin + '/#/authenticated')
            const realmUrl = encodeURIComponent(window.location.origin)
            let base = 'https://steamcommunity.com/openid/login
        ?openid.mode=checkid_setup
        &openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0
        &openid.ns.sreg=http%3A%2F%2Fopenid.net%2Fextensions%2Fsreg%2F1.1
        &openid.sreg.optional=nickname%2Cemail%2Cfullname%2Cdob%2Cgender%2Cpostcode%2Ccountry%2Clanguage%2Ctimezone
        &openid.ns.ax=http%3A%2F%2Fopenid.net%2Fsrv%2Fax%2F1.0
        &openid.ax.mode=fetch_request
        &openid.ax.type.fullname=http%3A%2F%2Faxschema.org%2FnamePerson
        &openid.ax.type.firstname=http%3A%2F%2Faxschema.org%2FnamePerson%2Ffirst
        &openid.ax.type.lastname=http%3A%2F%2Faxschema.org%2FnamePerson%2Flast
        &openid.ax.type.email=http%3A%2F%2Faxschema.org%2Fcontact%2Femail
        &openid.ax.required=fullname%2Cfirstname%2Clastname%2Cemail
        &openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select
        &openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select
        &openid.return_to={RETURN}
        &openid.realm={REALM}'
            const url = base.replace('{RETURN}', returnUrl).replace('{REALM}', realmUrl)
            window.location = url
        }
        {
            var steamId = this.$route.query['openid.identity'].replace('https://steamcommunity.com/openid/id/','')
            var accountId = convertor.to32(steamId)
            localStorage.setItem("accountId", accountId);
            this.$router.push({ name: 'profile' });
        }


        https://steamcommunity.com/openid/login?openid.mode=checkid_setup&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.ns.sreg=http%3A%2F%2Fopenid.net%2Fextensions%2Fsreg%2F1.1&openid.sreg.optional=nickname%2Cemail%2Cfullname%2Cdob%2Cgender%2Cpostcode%2Ccountry%2Clanguage%2Ctimezone&openid.ns.ax=http%3A%2F%2Fopenid.net%2Fsrv%2Fax%2F1.0&openid.ax.mode=fetch_request&openid.ax.type.fullname=http%3A%2F%2Faxschema.org%2FnamePerson&openid.ax.type.firstname=http%3A%2F%2Faxschema.org%2FnamePerson%2Ffirst&openid.ax.type.lastname=http%3A%2F%2Faxschema.org%2FnamePerson%2Flast&openid.ax.type.email=http%3A%2F%2Faxschema.org%2Fcontact%2Femail&openid.ax.required=fullname%2Cfirstname%2Clastname%2Cemail&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.return_to=http://localhost:7071/api/authorize&openid.realm=http://localhost:7071/
        
        http://localhost:7071/api/authorize
        ?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0
        &openid.mode=id_res&openid.op_endpoint=https%3A%2F%2Fsteamcommunity.com%2Fopenid%2Flogin
        &openid.claimed_id=https%3A%2F%2Fsteamcommunity.com%2Fopenid%2Fid%2F76561197973295540
        &openid.identity=https%3A%2F%2Fsteamcommunity.com%2Fopenid%2Fid%2F76561197973295540
        &openid.return_to=http%3A%2F%2Flocalhost%3A7071%2Fapi%2Fauthorize
        &openid.response_nonce=2020-10-01T15%3A41%3A01Zwo%2FUG8eazqUld8PN8ZDYzEcE%2FLc%3D
        &openid.assoc_handle=1234567890
        &openid.signed=signed%2Cop_endpoint%2Cclaimed_id%2Cidentity%2Creturn_to%2Cresponse_nonce%2Cassoc_handle
        &openid.sig=e2HVmrMm1SlCmtSbJEDwvDE%2F4aw%3D
        */
    }
}
