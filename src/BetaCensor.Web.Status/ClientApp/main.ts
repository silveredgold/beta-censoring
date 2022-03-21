import { createApp } from 'vue'
import App from './App.vue'
// General Font
import 'vfonts/Lato.css'
// Monospace Font
import 'vfonts/FiraCode.css'
import { backendProviderPlugin } from './backend-plugin'
import { requestPlugin } from './request-plugin'

createApp(App)
.use(backendProviderPlugin)
.use(requestPlugin)
.mount('#app')
