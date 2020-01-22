import { connectivityModule } from "./connectivity.module";

function bannerStorage(bannerId){
    debugger;
    let localStorageList = JSON.parse(global.localStorage.getItem("sb_banner"));
    if(localStorageList == null){
        localStorageList = JSON.stringify([bannerId]);
        global.localStorage.setItem("sb_banner",localStorageList);  
    }else{
        localStorageList = JSON.stringify(localStorageList.push(bannerId));
        global.localStorage.setItem("sb_banner",localStorageList); 
    }
}

function BannerData(objInit){
    this.id = objInit.id ;
    this.title = objInit.title;
    this.subTitle = objInit.subTitle;
    this.expirationDate = objInit.expirationDate;
    this.coupon = objInit.coupon;
}

function createBannerData(objInit){
    return new BannerData(objInit);
}

function getBannerParams() {
    return connectivityModule.http.get("HomePage/banner").then(obj=>{
        if(obj.data == null){
            return null;
        }else{
            return createBannerData(obj.data);
        }
    });
}

export default {
    getBannerParams,
    bannerStorage
}
