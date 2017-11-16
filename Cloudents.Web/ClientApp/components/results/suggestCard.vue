<template>
    <button type="button" class="suggest">
        <v-container >
            <v-layout row>
                <v-flex>
                    <div class="logo-wrap">
                        <logo></logo>
                    </div>
                </v-flex>
                <v-flex class="text">
                    I found some sweet for you.  You can thank me later:)
                </v-flex>
            </v-layout>
        </v-container>
    </button>
    <!--<button type="button" class="mt-2 mb-2 pa-2 suggest">

        I found some sweet  for you. You can thank me later:)

        asda
    </button>-->
    <!--{{props.name}}-->
</template>
<script>
    import logo from "./svg/sb-logo-copy.svg";
    export default {
        components: { logo }
    }
</script>
<style scoped lang="less">
    .logo-wrap {
        width: 64px;
        height: 64px;
        background: #000;
        border-radius: 50%;
        display: flex;
        justify-content: center;
        flex-direction: column;
    }

    svg {
        height: 28px;
    }
    .text {
        display: flex;
        justify-content: center;
        flex-direction: column;
    } 
    .suggest {
        background: #FFF;
        width: 100%;
        //cursor: pointer;
        border-radius: 4px;
        color: #000;
    }
</style>
