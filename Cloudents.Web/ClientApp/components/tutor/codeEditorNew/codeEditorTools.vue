<template>
    <div class="nav-container">
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" class="nav-action" >
                        <v-switch class="switch" color="primary" v-model="themeMode" label="light Theme" hide-details />
                    </button>
                </template>
                <span>Choose Theme</span>
            </v-tooltip>
        </div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';

export default {
    methods:{
        ...mapActions(['updateThemeMode']),
    },
    computed:{
        ...mapGetters(['getIsDarkTheme']),
        themeMode:{
            get(){
                return !this.getIsDarkTheme
            },
            set(val){
                this.updateThemeMode(!val)
            }
        }
    }
}
</script>

<style lang="less">
.nav-container {
            .nav-action {
                .switch{
                    margin: 0;
                }
            }
        }
</style>
