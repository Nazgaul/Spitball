<template>
    <div class="splash">
        <div class="newIsraeli-close" :class="{'rtl': isRtl}" @click.prevent="closeDialog()"><v-icon>sbf-close</v-icon></div>
        <div class="newIsraeli-spitball">Spitball</div>
        <div class="newIsraeli-title">הדרך לפתור שיעורי בית השתדרגה</div>
        <div class="newIsraeli-explain">הנקודת שלכם שוות Gift Cards</div>
        <div class="newIsraeli-buy-thumb">
            <img src="./img/buymethumb.png" alt="">
        </div>
        <div class="newIsraeli-take-me" @click.prevent="letGo()"><span>קחו אותי לאתר החדש</span></div>
        <div class="newIsraeli-leave-me" @click.prevent="goToHebSite()" :class="{'rtl': isRtl}">חזרה לאתר הישן <v-icon :class="{'rtl': isRtl}">sbf-full-arrow</v-icon> </div>
        <div class="newIsraeli-stats" :class="{'rtl': isRtl}">
            <div class="newIsraeli-students">
                <div class="stats-bold">395,110</div>
                <div>סטודנטים</div>
            </div>
            <div class="border-sep"></div>
            <div class="newIsraeli-documents">
                <div class="stats-bold">262,993</div>
                <div>מסמכי לימוד</div>
            </div>
            <div class="border-sep"></div>
            <div class="newIsraeli-courses">
                <div class="stats-bold">416,531</div>
                <div>קורסים</div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
props:{
    closeDialog:{
        required:true,
        type: Function
    }
},
data(){
    return{
        isRtl: global.isRtl
    }
},
methods:{
    goToHebSite(){
        window.location = "http://heb.spitball.co";
    },
    letGo(){
        global.localStorage.setItem("sb-newIsraei", "true"); 
        this.closeDialog();
        this.$router.push({path: '/register'});
    }
}
}
</script>

<style lang="less" src="./newIsraeliPop.less"></style>
