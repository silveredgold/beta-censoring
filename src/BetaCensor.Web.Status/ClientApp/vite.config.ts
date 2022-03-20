import { defineConfig } from 'vite'
import { fileURLToPath, URL } from "url";
// import { visualizer } from "rollup-plugin-visualizer";
import vue from '@vitejs/plugin-vue'

export default defineConfig({
    plugins: [vue()],
    base: '/dist/',
    resolve: {
        alias: {
            "@": fileURLToPath(new URL("./src", import.meta.url)),
            "#": "@silveredgold/beta-shared"
        },
    },
    build: {
        outDir: '../wwwroot/dist',
        emptyOutDir: true,
        manifest: true,
        rollupOptions: {
            input: {
                main: './main.ts',
                config: './config.ts'
            }
        }
    },
    server: {
        hmr: {
            protocol: 'ws'
        }
    }
})