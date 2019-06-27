<template>
    <section class="registerPage">
        <div class="leftSection">
            <p>Everything you’ll need to master your studies
            </p>
        </div>
        <div class="stepsSections">
            <div>
                <component :is="`${currentStep}`"/>
            </div>
            <!-- <div class="progressContainer">
               <span>.</span>....
            </div> -->
        </div>
    </section>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import getStarted from '../components/getStarted.vue'
import setEmailPassword from '../components/setEmailPassword.vue'

export default {
    components:{
        getStarted,
        setEmailPassword,
        },
    computed: {
        ...mapGetters(['getCurrentLoginStep',]),
        currentStep(){
            return this.getCurrentLoginStep
        }
    },
}
</script>
<style lang="less" src="./registerPage.less"/>