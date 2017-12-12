<template>
    <button type="button" class="suggest">
        <v-container>
            <v-layout row>
                <v-flex class="image-wrapper">
                    <div class="logo-wrap">
                        <logo></logo>
                    </div>
                </v-flex>

                <v-flex class="text-wrapper">
                    <v-layout class="wrapper" align-content-space-between column>
                        <v-flex class="text">
                            I found some sweet {{name}} for you.  You can thank me later:)
                        </v-flex>
                        <v-flex class="button-text">
                            <div>Show me</div>
                        </v-flex>
                    </v-layout>
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
    import logo from "./svg/sb-logo.svg";
    export default {
        components: { logo },

        props: {
            name: { type: String }
        }
    }
</script>
<style src="./suggestCard.less" lang="less" scoped></style>