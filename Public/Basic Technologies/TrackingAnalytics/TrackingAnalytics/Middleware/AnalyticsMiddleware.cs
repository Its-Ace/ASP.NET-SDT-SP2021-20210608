using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MySqlConnector;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackingAnalytics.Core.Models;
using TrackingAnalytics.Infrastructure.Context;

namespace TrackingAnalytics.middlewares
{
	public class AnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecordContext _context = null; 
        private static RecordModel start = null;
        private string _connectionString { get; set; }
	
		private readonly IConfiguration _configuration;

        public AnalyticsMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            RecordContext context
            )
        {
            _context = context;
            _next = next;
            _logger = loggerFactory.CreateLogger<AnalyticsMiddleware>();
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AnalyticalConnection");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (!Regex.IsMatch(
                    context.Request.Path,
                    @"(^\/css\/.*)|(^\/fonts\/.*)|(^\/images\/.*)|(^\/js\/.*)|(^\/lib\/.*)|(^\/favicon(-.*|.ico$))|(^\/robots.txt$)|(^\/rss.xml$)",
                    RegexOptions.IgnoreCase
                    ))
                {
                    start = new RecordModel();
                    start.Time = DateTime.Now;
                    start.path = context.Request.Path.ToString();
                    start.query = context.Request.QueryString.ToString();
                    start.referer = context.Request.Headers[HeaderNames.Referer].ToString();
                    start.userAgent = context.Request.Headers[HeaderNames.UserAgent].ToString();

                    _context.Add(start);
                    _context.SaveChanges();

                    // only requests for "good" resources get processed here
                    using (MySqlConnection sqlConn = new MySqlConnection(_connectionString))
                    {
                        sqlConn.Open();

                        MySqlCommand cmd = new MySqlCommand("INSERT INTO analytics(dt, ip, path, query, referer, ua) VALUES(@dt, @ip, @path, @query, @referer, @ua);", sqlConn);
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ip", context.Connection.RemoteIpAddress.ToString());
                        cmd.Parameters.AddWithValue("@path", context.Request.Path.ToString());
                        cmd.Parameters.AddWithValue("@query", context.Request.QueryString.ToString());
                        cmd.Parameters.AddWithValue("@referer", context.Request.Headers[HeaderNames.Referer].ToString());
                        cmd.Parameters.AddWithValue("@ua", context.Request.Headers[HeaderNames.UserAgent].ToString());
                        cmd.ExecuteNonQuery();
                        _logger.LogWarning("The Statements were executed");
                    }
                }
                else
                {
                    //All others requests came here 
                    using (MySqlConnection sqlConn = new MySqlConnection(_connectionString))
                    {
                        sqlConn.Open();

                        MySqlCommand cmd = new MySqlCommand("INSERT INTO extras(dt, ip, path, query, referer, ua) VALUES(@dt, @ip, @path, @query, @referer, @ua);", sqlConn);
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ip", context.Connection.RemoteIpAddress.ToString());
                        cmd.Parameters.AddWithValue("@path", context.Request.Path.ToString());
                        cmd.Parameters.AddWithValue("@query", context.Request.QueryString.ToString());
                        cmd.Parameters.AddWithValue("@referer", context.Request.Headers[HeaderNames.Referer].ToString());
                        cmd.Parameters.AddWithValue("@ua", context.Request.Headers[HeaderNames.UserAgent].ToString());
                        cmd.ExecuteNonQuery();
                        _logger.LogWarning("Extra file requests were found");
                    }
                }

                if (Regex.IsMatch(
                    context.Request.Headers[HeaderNames.UserAgent],
                    @"(\/bot)|(bot\/)",
                    RegexOptions.IgnoreCase
                    ))
                {
                    // do something with it, or actually simply don't analyze such requests
                    using (MySqlConnection sqlConn = new MySqlConnection(_connectionString))
                    {
                        sqlConn.Open();

                        MySqlCommand cmd = new MySqlCommand("INSERT INTO extra_requests(dt, ip, path, query, referer, ua) VALUES(@dt, @ip, @path, @query, @referer, @ua);", sqlConn);
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ip", context.Connection.RemoteIpAddress.ToString());
                        cmd.Parameters.AddWithValue("@path", context.Request.Path.ToString());
                        cmd.Parameters.AddWithValue("@query", context.Request.QueryString.ToString());
                        cmd.Parameters.AddWithValue("@referer", context.Request.Headers[HeaderNames.Referer].ToString());
                        cmd.Parameters.AddWithValue("@ua", context.Request.Headers[HeaderNames.UserAgent].ToString());
                        cmd.ExecuteNonQuery();
                        _logger.LogWarning("Bots requests were found");
                    }

                }
			


            }
            catch (Exception ex)
            {
                // we don't care much about exceptions in analytics
                _logger.LogError($"Some error in analytics middleware. {ex.Message}");
            }

            await _next(context);
        }
    }

    public static class AnalyticsMiddlewareExtensions
    {
        //Relation with (app)
        public static IApplicationBuilder UseAnalytics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnalyticsMiddleware>();
        }
    }
}
