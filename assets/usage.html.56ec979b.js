import{r as n,o as s,c as a,a as t,b as o,w as c,F as l,d,e}from"./app.96becb97.js";import{_ as h}from"./plugin-vue_export-helper.21dcd24c.js";const u={},_=d('<h1 id="usage" tabindex="-1"><a class="header-anchor" href="#usage" aria-hidden="true">#</a> Usage</h1><p>In it&#39;s current form, you don&#39;t really <em>use</em> Beta Censoring in the traditional sense. You won&#39;t directly interact with it and it doesn&#39;t (currently) have its own interface. The current version of Beta Censoring is explicitly a <strong>server</strong>. That means you leave it running and other apps (we call them &#39;clients&#39;) can interact with it, requesting images to be censored.</p><p>That does mean that you need to leave it running though, as clients will need to contact the running server to request censoring.</p><blockquote><p>If you&#39;re a more advanced user, Beta Censoring can be configured as a Windows Service or Systemd service to have the server running, but not visible.</p></blockquote><h2 id="clients" tabindex="-1"><a class="header-anchor" href="#clients" aria-hidden="true">#</a> Clients</h2>',5),g=e("At this time, the only "),m=t("em",null,"known",-1),p=e(" Beta Censoring client is "),f={href:"https://silveredgold.github.io/beta-protection/#/",target:"_blank",rel:"noopener noreferrer"},v=e("Beta Protection"),b=e(", the browser extension for live censoring images while you browse."),w=e("Beta Censoring can accept any client though, and if you're interested in integrating it with other tools or building your own client, the "),y=e("developer documentation"),x=e(" includes more details.");function k(B,C){const r=n("ExternalLinkIcon"),i=n("RouterLink");return s(),a(l,null,[_,t("p",null,[g,m,p,t("strong",null,[t("a",f,[v,o(r)])]),b]),t("p",null,[w,o(i,{to:"/content/developers.html"},{default:c(()=>[y]),_:1}),x])],64)}var L=h(u,[["render",k]]);export{L as default};