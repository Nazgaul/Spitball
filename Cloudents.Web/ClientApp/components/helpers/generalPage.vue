<template functional>
    <v-flex offset-lg1 class="mt-5">
        <slot></slot>
        <div class="loader" v-if="props.isLoading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <v-container fluid v-else>
            <v-layout row>
                <v-flex class="sec-result">
                    <slot name="title"><h5>
                        {{props.title}}
                    </h5></slot>
                    <slot name="options" >

                    </slot>
                </v-flex>
            </v-layout>
            <v-layout row>
                <v-flex class="sec-result">
                    <slot name="data">

                    </slot>
                </v-flex>
                <slot name="adsense"><v-flex class="ml-1 mt-1 hidden-md-only hidden-xs-only">

                    <adsense ad-client="ca-pub-1215688692145777"
                             ad-slot="3866041406"
                             ad-style="display: block; width:336px; height:280px;"
                             ad-format="auto" class="mb-2">
                    </adsense>
                    <adsense ad-client="ca-pub-1215688692145777"
                             ad-slot="5547053037"
                             ad-style="display: block; width:336px;height:280px">
                    </adsense>
                </v-flex></slot>
            </v-layout>
        </v-container>
    </v-flex>
</template>
