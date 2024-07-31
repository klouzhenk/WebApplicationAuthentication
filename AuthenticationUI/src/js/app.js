import { saveAsFile } from "./modules/FileUtil.js";
import { clearLocalStorage } from "./modules/Storage.js";

export function SaveAsFile(filename, bytesBase64) {
    return saveAsFile(filename, bytesBase64);
}

export function ClearLocalStorage(filename, bytesBase64) {
    return clearLocalStorage();
}