<template>
    <div class="uni-select-component">
        <!-- <img src="./assets/img/group.png" alt=""> -->
        <div class="back-img">
            <div class="close-container">
                <v-icon>sbf-close</v-icon>
            </div>
        </div>
        <div class="select-university-container">
            <div class="title-container">
                Set your school & classes
            </div>
            <div class="explain-container">
                By setting this up you will get the best questions & study info fit for your shool & classes
            </div>
            
            <div class="select-school-container">
                <input type="text" placeholder="Select Your School"> 
                <v-icon>sbf-arrow-right</v-icon>
            </div>

            <div class="skip-container">
                <a href="#">Skip for now</a> 
            </div>
        </div>
    </div>
</template>

<script>
export default {

}
</script>

<style lang="less">
.uni-select-component{   
    display: flex;
    margin: 0 auto;
    justify-content: center;
        .back-img{
            background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0)), url('assets/img/group.png');
            position: absolute;
            height: 100%;
            width: 1280px;
            .close-container{
                position:absolute;
                right:0;
                margin-right:37px;
                margin-top:36px;
                cursor: pointer;
                i{
                    font-size: 18px;
                    color:#fff;
                    opacity: 0.45;
                }
            }
        }
    
    .select-university-container{
            width: 499px;
            height: 516px;
            opacity: 0.91;
            border-radius: 4px;
            box-shadow: 0 1px 17px 0 rgba(16, 27, 147, 0.39);
            background-image: linear-gradient(140deg, #4b8ffe, #4452fc);
            background-repeat: no-repeat;
            margin-top: 26px;
            margin-right: 18%;
            padding-left: 32px;
            padding-right: 32px;
            .title-container{
                width: 100%;
                height: 120px;
                font-size: 40px;
                font-weight: 600;
                color: #ffffff;
                margin-top: 32px;
                line-height: 55px;
                font-family: 'Fira Sans', sans-serif;
            }
            .explain-container{
                width: 100%;
                height: 56px;
                margin-top: 82px;
                font-family: 'Fira Sans', sans-serif;
                font-size: 18px;
                font-weight: 300;
                font-style: normal;
                font-stretch: normal;
                line-height: 1.56;
                letter-spacing: normal;
                color: #ffffff;
            }
            .select-school-container{
                margin-top:20px;
                position:relative;
                input{
                    font-size: 16px;
                    width: 100%;
                    height: 56px;
                    border-radius: 4px;
                    background-color: #ffffff;
                    padding-left:16px;
                    outline: none;
                }
                i{
                    position: absolute;
                    right: 20px;
                    top: 24px;
                    font-size: 12px;
                    font-weight: bold;
                    opacity: 0.5;
                }
            }
            .skip-container{
                margin-top:84px;
                a{
                    height: 18px;
                    font-family: 'Open Sans';
                    font-size: 13px;
                    font-weight: normal;
                    font-style: normal;
                    font-stretch: normal;
                    line-height: normal;
                    letter-spacing: -0.3px;
                    color: rgba(255, 255, 255, 0.54);
                }
            }
        }
}
</style>
