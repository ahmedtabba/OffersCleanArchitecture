using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Posts.Queries;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class PostHelper
{
    public static async Task<bool> IsPostActive(Guid postId, IPostRepository postRepository)
    {
        var post = await postRepository.GetByIdAsync(postId);
        // post is IsLiven if it is not null,IsActive,have StartDate then 
        //                                                      if dose not have EndDate
        //                                                      or have EndDate and today is between StartDate and EndDate
        if (post != null && post.IsActive && post.StartDate != null)
        {
            if (post.EndDate == null)
            {
                return await Task.FromResult(true);
            }
            else
            {
                var today = DateTime.UtcNow;
                if (today >= post.StartDate && today <= post.EndDate)
                {
                    return await Task.FromResult(true);
                }
                else
                    return await Task.FromResult(false);
            }
        }
        else
          return await Task.FromResult(false);
    }
    public static  bool IsPostActive(Post post)
    {
        // post is IsLiven if it is not null,IsActive,have StartDate then 
        //                                                      if dose not have EndDate
        //                                                      or have EndDate and today is between StartDate and EndDate
        if (post != null && post.IsActive && post.StartDate != null)
        {
            if (post.EndDate == null)
            {
                return true;
            }
            else
            {
                var today = DateTime.UtcNow;
                if (today >= post.StartDate && today <= post.EndDate)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        else
            return false;
    }


    public static async Task ConvertDateTimeToUTC(Offers.CleanArchitecture.Domain.Entities.Post post, IUserContext userContext, ICountryRepository countryRepository)
    {
        // get required information to reach TimeZoneInfo of country TimeZoneId
        var countryIdOfUser = userContext.GetCountryIdOfUser();
        var country = await countryRepository.GetByIdAsync(countryIdOfUser);
        var tzId = country.TimeZoneId;
        var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        // convert datetime to UTC before saving in DB
        post.PublishDate = post.PublishDate.ConvertSpecifiedDateTimeToUTC(tzInfo);
        post.StartDate = post.StartDate.ConvertSpecifiedDateTimeToUTC(tzInfo);
        post.EndDate = post.EndDate.ConvertSpecifiedDateTimeToUTC(tzInfo);
    }


    // this overload is used in Get queries that handle unauthorized requests
    public static async Task ConvertDateTimeToTimeZone(PostBaseDto post, /*IUserContext userContext,*/ ICountryRepository countryRepository, Guid userCountryId)
    {
        // get required information to reach TimeZoneInfo of country TimeZoneId
        //var countryIdOfUser = userContext.GetCountryIdOfUser();
        var country = await countryRepository.GetByIdAsync(userCountryId);
        var tzId = country.TimeZoneId;
        var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        // convert datetime From UTC before sending to FrontEnd 
        post.PublishDate = post.PublishDate.ConvertSpecifiedDateTimeToTimeZoneDate(tzInfo);
        post.StartDate = post.StartDate.ConvertSpecifiedDateTimeToTimeZoneDate(tzInfo);
        post.EndDate = post.EndDate.ConvertSpecifiedDateTimeToTimeZoneDate(tzInfo);
    }


    // this overload is used in Get queries that handle authorized requests
    public static async Task ConvertDateTimeToTimeZone(PostBaseDto post, IUserContext userContext, ICountryRepository countryRepository)
    {
        // get required information to reach TimeZoneInfo of country TimeZoneId
        var countryIdOfUser = userContext.GetCountryIdOfUser();
        var country = await countryRepository.GetByIdAsync(countryIdOfUser);
        var tzId = country.TimeZoneId;
        var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        // convert datetime From UTC before sending to FrontEnd 
        post.PublishDate = post.PublishDate.ConvertSpecifiedDateTimeToTimeZoneDate(tzInfo);
        post.StartDate = post.StartDate.ConvertSpecifiedDateTimeToTimeZoneDate(tzInfo);
        post.EndDate = post.EndDate.ConvertSpecifiedDateTimeToTimeZoneDate(tzInfo);
    }

}
