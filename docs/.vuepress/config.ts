import { defineUserConfig } from 'vuepress'
import type { DefaultThemeOptions } from 'vuepress'

export default defineUserConfig<DefaultThemeOptions>({
  // site config
  lang: 'en-US',
  title: 'Beta Censoring',
  description: 'On-demand censoring of NSFW images just for betas',
  base: '/beta-censoring/',
  // theme and its config
  theme: '@vuepress/theme-default',
  themeConfig: {
    logo: '/siteIcon.png',
    repo: 'silveredgold/beta-censoring',
    docsDir: 'docs',
    navbar: [
        // NavbarItem
        {
          text: 'Introduction',
          link: '/',
        },
        // NavbarGroup
        {
          text: 'User Guide',
          children: ['/content/installation.md', '/content/usage', '/content/configuration', '/content/performance', '/content/beta-safety'],
        },
        // string - page file path
        {
            text: 'Developers',
            link: '/content/developers'
        },
        // {
        //     text: 'GitHub',
        //     link: 'https://github.com/silveredgold/beta-censoring/'
        // }
      ],
  },
})