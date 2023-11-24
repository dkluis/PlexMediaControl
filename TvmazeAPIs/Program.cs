using Common_Lib;
using Web_Lib;
using Web_Lib.DTOs;

var appInfo = new AppInfo("PlexMediaControl", "EF-Console");
var tvmApIs = new WebApi(appInfo);
//var result1 = tvmApIs.GetShow(71334);
// var result2 = tvmApIs.GetEpisode(2624641);
// var result3 = tvmApIs.GetEpisodesByShow(71334);
// var result4 = tvmApIs.GetEpisodeMarks(2690856);
var result5 = tvmApIs.GetAllMyEpisodes();
var h       = "";
