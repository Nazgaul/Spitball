<template>
    <div class="change-country-container">
        <h1>Change User Country</h1>
        <div class="user-inputs-container">
            <v-text-field solo class="user-input-text" placeholder="User Id..." type="text" v-model="userId"/>
        </div>
        <div class="select-type-container">
            <v-select attach=""
                      class="select-type-input"
                      solo
                      v-model="userCountry"
                      :items="countryList"
                      label="Select country"
            ></v-select>

        </div>
        <div class="change-container">
            <v-btn round color="#78c967" class="change-button" :class="{'disabled': lock}" @click="change()">Change</v-btn>
        </div>
    </div>
</template>

<script>
import { setUserCountry } from './changeCountryService.js'

export default {
    data(){
        return {
            userCountry: "Us",
            userId: "",
            lock: false,
            countryList: ['Us', 'Il', 'In']
        }
    },
    methods:{
        change(){
            if(this.userId === "") return;
            this.lock = true;
            let senObj = {
                id: this.userId,
                country: this.userCountry
            }
            setUserCountry(senObj).then(()=>{
                this.$toaster.success(`Country Changed - logout to effect!`);
                this.userCountry = "Us";
                this.userId = "";
            },(err)=>{
                console.log(err);
                this.$toaster.error(`Error: couldn't change country`)
            }).finally(()=>{
                this.lock = false;
            })
        }
    }
}
</script>

<style lang="scss" scoped>
.change-country-container{
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    height: 100%;
    min-height: 350px;
    .user-inputs-container{
            display:flex;
            flex-direction: column;
            justify-content: center;
            .user-input-text{
                width: 345px;
            }
        }
    .select-type-container{
        width: 345px;
            .select-type{
                border: none;
                border-radius: 25px;
                height: 25px;
                margin-top: 10px;
                width: 90px;
                padding: 5px;
                outline: none;
            }
        }
        .change-container{
            margin-top: 15px;
            .change-button{
                /*cursor: pointer;*/
                /*border:none;*/
                /*outline: none;*/
                /*background-color: #78c967;*/
                /*border-radius: 25px;*/
                /*height:25px;*/
                /*width: 75px;*/
                &.disabled{
                    background-color: #aeabab;
                    pointer-events: none;
                    color: #837d7d;
                }
            }
    }
}

</style>
