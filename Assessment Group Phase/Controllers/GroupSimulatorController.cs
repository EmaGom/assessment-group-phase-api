using Assessment.Group.Phase.Managers;
using Assessment.Group.Phase.Models;
using Assessment.Group.Phase.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupSimulatorController : ControllerBase
    {
        private readonly ILogger<GroupSimulatorController> _logger;
        private readonly IGroupManager _groupManager;
        private readonly ITeamManager _teamManager;

        public GroupSimulatorController(ILogger<GroupSimulatorController> logger, IGroupManager groupManager, ITeamManager teamManager)
        {
            _logger = logger;
            _groupManager = groupManager;
            _teamManager = teamManager;
        }

        /// <summary>
        /// Get a new simulated group
        /// </summary>
        /// <returns>A group, which represent a new simulation</returns>
        [HttpGet]
        [Route("[action]", Name = nameof(GetGroup))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGroup()
        {
            try
            {
                _teamManager.SetTeams();
                _groupManager.CreateGroup();
                _groupManager.SimulateMatchDay(1);
                _groupManager.SimulateMatchDay(2);
                _groupManager.SimulateMatchDay(3);

                GroupEntity response = _groupManager.GetGroup();
                return Ok(response);
            }
            catch (Exception exception)
            {
                return exception is BadRequestException ?
                        BadRequest(exception.Message) :
                        StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        /// <summary>
        /// Save group with team statics
        /// </summary>
        /// <returns>GroupId</returns>
        [HttpPost]
        [Route("[action]", Name = nameof(SaveGroup))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SaveGroup(GroupEntity group)
        {
            try
            {
                if(group == null)
                    return BadRequest(ApiResponseErrorMessages.BadRequest);

                GroupEntity result = _groupManager.SaveGroup(group);

                return StatusCode(StatusCodes.Status201Created, result.Id);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        /// <summary>
        /// Get group simulations history
        /// </summary>
        /// <param name="last">last groups simualted, if null takes last 10</param>
        /// <returns>A list of groups</returns>
        [HttpGet]
        [Route("[action]", Name = nameof(GetGroupsHistory))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGroupsHistory([FromQuery] int last = 10)
        {
            try
            {
                IList<GroupEntity> groups = _groupManager.GetGroupsHistory(last);
                return Ok(groups);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}
