import { GlobalThemeOverrides } from "naive-ui";

export const themeOverrides: GlobalThemeOverrides = { common: { fontWeightStrong: '600', primaryColor: '#61aeee' }, Result: { lineHeight: '1.1', titleFontSizeSmall: '24', iconSizeSmall: '48px' } };

export type CensoringRecord = {
    censoring: {
        censoringTime: string
    },
    classes: string[],
    id: string,
    inference: {
        modelRunTime: string,
        imageLoadTime: string,
        tensorLoadTime: string,
        modelName: string
    }
};