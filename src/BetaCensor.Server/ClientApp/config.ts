import { createApp } from 'vue'
import Config from './Config.vue'
// General Font
import 'vfonts/Lato.css'
// Monospace Font
import 'vfonts/FiraCode.css'
import { backendProviderPlugin } from './backend-plugin'
import { requestPlugin } from './request-plugin'

createApp(Config)
.use(backendProviderPlugin)
.use(requestPlugin)
.mount('#app')
