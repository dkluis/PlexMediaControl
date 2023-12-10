using Common_Lib;

using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using Web_Lib;
using Web_Lib.DTOs;

var appInfo = new AppInfo("PlexMediaControl", "EF-Console");
var tvmApIs = new WebApi(appInfo);

//var show = tvmApIs.GetShow(71334);
// var episode = tvmApIs.GetEpisode(2624641);
// var showEpisodes = tvmApIs.GetEpisodesByShow(71334);
var showEpochs = tvmApIs.GetShowUpdateEpochs("day");

// var result4 = tvmApIs.GetEpisodeMarks(2690856);
//var result5 = tvmApIs.GetAllMyEpisodes();
var h = "";
