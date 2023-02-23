<script lang="ts">
  import RenderObject from "./RenderObject.svelte";
  export let data: unknown;
  export let path: string[];
  export let maxLevel: number;
  let _data = data || {};
  let strPath = (path || []).join(".");
</script>

{#if data === null}
  <pre class="value keyword">null</pre>
  <pre class="metadata">{strPath}</pre>
{:else if typeof _data === "object"}
  {#if path.length > maxLevel}
    <a href="#{strPath}">
      <pre>{strPath}</pre>
    </a>
  {:else if _data instanceof Map}
    <pre class="value complex">Map()</pre>
    <pre class="metadata">{strPath}</pre>
  {:else if _data instanceof WeakMap}
    <pre class="value complex">WeakMap()</pre>
    <pre class="metadata">{strPath}</pre>
  {:else if _data instanceof Date}
    <pre class="value date">{_data.toLocaleString()}</pre>
    <pre class="metadata">{strPath}</pre>
  {:else}
    <RenderObject data={_data} {maxLevel} {path} />
  {/if}
{:else if typeof data === "bigint"}
  <pre class="value number bigint">{data}</pre>
  <pre class="metadata">{strPath}</pre>
{:else if typeof data === "number"}
  <pre class="value number">{data}</pre>
  <pre class="metadata">{strPath}</pre>
{:else if typeof data === "symbol"}
  <pre class="value symbol">{data.toString()}</pre>
  <pre class="metadata">{strPath}</pre>
{:else if typeof data === "string"}
  <pre class="value string">"{data}"</pre>
  <pre class="metadata">{strPath}</pre>
{:else if typeof data === "boolean"}
  <pre class="keyword boolean">{data}</pre>
  <pre class="metadata">{strPath}</pre>
{:else if typeof data === "function"}
  <pre class="function">{data}</pre>
  <pre class="metadata">{strPath}</pre>
{:else}
  <pre>Unknown: {String(data)}</pre>
  <pre class="metadata">{strPath}</pre>
{/if}
