<template>
      <video v-if="src"
             :id="uniqueID" 
             :tabindex="tabindex"
             :width="width" :height="height"
              class="azuremediaplayer amp-flush-skin amp-big-play-centered">
    </video>
</template>

<script src="./sbVideoPlayer.js"/>
 
<style lang="less">
    .azuremediaplayer {
        width: 100%;
    }
</style>