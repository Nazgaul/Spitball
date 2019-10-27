<template>
    <div class="headlineSection">
        <div class="headlineSearch">
            <div class="headlineTitles">
                <h1 class="hd-title">Improve your grades with help throughout your studies</h1>
                <h2 class="hd-subtitle mt-3">Access course specific study materials, talk to expert teachers or watch educational videos.</h2>
                <div class="hd-search mt-3">
                    <div class="search-input"></div>
                    <div class="search-btn">
                        Search
                    </div>
                </div>
            </div>
        </div>
        <FindTutorHeadlineStatus></FindTutorHeadlineStatus>
    </div>
</template>

<script>
import FindTutorHeadlineStatus from './findTutorComponents/FindTutor-headlineStatus.vue'
export default {
    components:{
        FindTutorHeadlineStatus
    }

}
</script>

<style lang="less">
    .headlineSection{
        width: 100%;
        height: 562px;
        background-image: url('../images/hpHeadline.png');
        background-repeat: no-repeat;
        background-size: cover;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        .headlineSearch{
            color: #ffffff;
            font-stretch: normal;
            font-style: normal;
            letter-spacing: normal;
            width: 100%;
            height: calc(~"100% - 62px");
            display: flex;
            align-items: center;
            max-width: calc(100% - 644px);
            margin: 0 auto;
            .headlineTitles{
                max-width: 536px;
                .hd-title{
                    font-size: 36px;
                    font-weight: bold;
                }
                .hd-subtitle{
                    font-size: 16px;
                    font-weight: normal;
                    padding-right: 60px;
                }
                .hd-search{
                    background: white;
                    width: 470px;
                    height: 50px;
                    border-radius: 12px;
                    display: flex;
                    .search-input{
                        flex: 2;
                        height: 100%;
                    }
                    .search-btn{
                        border-top-right-radius: 12px;
                        border-bottom-right-radius: 12px;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        height: 100%;
                        background-color: #4c59ff;
                        padding: 0 20px
                    }
                }
            }
        }
    }
</style>