using System;

namespace SlackAPI
{
    public class Team
    {
        public string domain;
        /// <summary>
        /// Supported domains emails can be registered from.
        /// </summary>
        /// TODO: Is this obsolete?
        public string email_domain;
        /// <summary>
        /// Supported domains emails can be registered from.
        /// </summary>
        public string[] email_domains;

        public string id;
        public long limit_ts;
        public DateTime LimitTimestamp { get { return new DateTime(1970, 1, 1).AddMilliseconds(limit_ts); } }
        public int msg_edit_window_mins;
        public string name;
        public bool over_storage_limit, sso;
        public TeamPreferences prefs;
        public string sso_required;
        public string sso_type;
        public string url;
        public SSOProvider[] sso_provider;
    }

    public class SSOProvider
    {
        public string name;
        public string type;
    }

    public class BotList
    {
        public Bot ATL_Homepage_News;
        public Bot Glengarry_Leads;
        public Bot LandingPages;
        public Bot SF_Homepage_News;
        public Bot Stats_Time;
        public Bot TEX_Homepage_News;
        public Bot TOR_Homepage_News;
        public Bot airbrake;
        public Bot amazon_sqs;
        public Bot appsignal;
        public Bot asana;
        public Bot bitbucket;
        public Bot blossom;
        public Bot bugbot;
        public Bot bugs;
        public Bot bugsnag;
        public Bot buildbox;
        public Bot circleci;
        public Bot butt66;
        public Bot code_climate;
        public Bot codeship;
        public Bot crashlytics;
        public Bot datadog;
        public Bot deployinator;
        public Bot disconnectedBot;
        public Bot dropbox;
        public Bot giphy;
        public Bot github;
        public Bot gosquared;
        public Bot hangouts;
        public Bot helpscout;
        public Bot heroku;
        public Bot honeybadger;
        public Bot hubot;
        public Bot ifttt;
        public Bot incoming_webhook;
        public Bot jenkins_ci;
        public Bot jira;
        public Bot librato;
        public Bot loadBot;
        public Bot magnum_ci;
        public Bot mailchimp;
        public Bot nagios;
        public Bot new_relic;
        public Bot ninefold;
        public Bot opsgenie;
        public Bot outgoing_webhook;
        public Bot pagerduty;
        public Bot papertrail;
        public Bot phabricator;
        public Bot pingdom;
        public Bot pivotaltracker;
        public Bot raygun;
        public Bot rollcall;
        public Bot rss;
        public Bot runscope;
        public Bot screenhero;
        public Bot semaphore;
        public Bot sentry;
        public Bot slackbot;
        public Bot sprintly;
        public Bot statuspageio;
        public Bot stripe;
        public Bot subversion;
        public Bot supportfu;
        public Bot travis;
        public Bot trello;
        public Bot twitter;
        public Bot userlike;
        public Bot zapier;
        public Bot zendesk;
        //public Bot ATL_Homepage_News,Glengarry_Leads,LandingPages,SF_Homepage_News,Stats_Time!,TEX_Homepage_News,TOR_Homepage_News,airbrake,amazon-sqs,appsignal,asana,bitbucket,blossom,bugbot,bugs,bugsnag,buildbox,circleci,butt66,code-climate,codeship,crashlytics,datadog,deployinator,disconnectedBot,dropbox,giphy,github,gosquared,hangouts,helpscout,heroku,honeybadger,hubot,ifttt,incoming-webhook,jenkins-ci,jira,librato,loadBot,magnum-ci,mailchimp,nagios,new-relic,ninefold,opsgenie,outgoing-webhook,pagerduty,papertrail,phabricator,pingdom,pivotaltracker,raygun,rollcall,rss,runscope,screenhero,semaphore,sentry,slackbot,sprintly,statuspageio,stripe,subversion,supportfu,travis,trello,twitter,userlike,zapier,zendesk;
    }
}
