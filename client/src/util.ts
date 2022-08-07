const __DEBUG__ = false;

export const dbg = (...data: any[]) => {
    if (__DEBUG__) {
        console.debug(...data);
    }
  }
  
  export const dbgLog = (...data: any[]) => {
    if (__DEBUG__) {
      console.log(...data);
    }
  }
  
  export const dbgTime = (label: string, id?: string) => {
    if (__DEBUG__) {
      console.time(id ? `${label}:${id}` : label);
    }
  }
  
  export const dbgTimeEnd = (label: string, id?: string) => {
    if (__DEBUG__) {
      console.timeEnd(id ? `${label}:${id}` : label);
    }
  }