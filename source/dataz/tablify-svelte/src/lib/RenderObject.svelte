<svelte:options tag="tablify-render-object" />
<script lang="ts">
  import Dispatch from "./Dispatch.svelte";
  export let data: object;
  export let maxLevel: number;
  export let path: string[];
  let keys = [...Object.keys(data), ...Object.getOwnPropertySymbols(data)];
  let strPath = (path || []).join(".");
  type ToggleState = "open" | "closed";
  let toggleState: Record<string | symbol, ToggleState> = {};
  function toggle(key: string | symbol) {
    const current = toggleState[key];
    toggleState[key] = current == null || current == "open" ? "closed" : "open";
  }
</script>

{#if keys.length < 1}
  <pre>[]</pre>
  <pre>{strPath}</pre>
{:else}
  <table>
    <thead>
      <tr>
        <th>Property</th>
        <th>Value</th>
      </tr>
    </thead>
    <tbody>
      {#each keys as key}
        <tr>
          <td>
            <span>{key.toString()}</span>
            <button type="button" on:click={(e) => toggle(key)}>T</button>
          </td>
          <td>
            {#if toggleState[key] == null || toggleState[key] == "open"}
              <div>
                <Dispatch
                  {maxLevel}
                  data={data[key]}
                  path={[...path, key.toString()]}
                />
              </div>
            {:else}
              <div style="display:none;">
                <Dispatch
                  {maxLevel}
                  data={data[key]}
                  path={[...path, key.toString()]}
                />
              </div>
            {/if}
          </td>
        </tr>
      {/each}
    </tbody>
  </table>
{/if}
